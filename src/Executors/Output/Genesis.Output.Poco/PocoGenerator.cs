using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genesis.Cli;
using Genesis.Output;

#nullable enable 

namespace Genesis.Output.Poco
{
    public class PocoGenerator : OutputExecutor
    {
        public override string CommandText => "poco";
        public override string Description => "Generates Plain Old C# Object .cs files with optional Suffix i.e. Dto";
        public override string FriendlyName => "POCO+Suffix Generator";

        public PocoConfig Config { get; set; } = new PocoConfig();

        protected override void OnInitialized()
        {
            Config = (PocoConfig)Configuration;
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputGenesisExecutionResult();

            if (!Directory.Exists(Config.OutputPath)) //TODO: Worry about the output path in the OutputGenerator base
                Directory.CreateDirectory(Config.OutputPath);

            await DepositDependencies();

            foreach (var obj in genesis.Objects)
                await ExecuteGraph(obj);

            result.Success = true;

            return result;
        }

        protected override GenesisDependency OnBeforeWriteDependency(object sender, DependencyEventArgs e)
        {
            string replaceTokens(string input)
               => input.Replace(Tokens.Namespace, Config.Namespace)  //TODO: Make more base level config properties so this can be global-er
                       .Replace(Tokens.ObjectName, e.Dependency.ObjectName)
                       .Replace(Tokens.ObjectBaseClass, Config.ObjectBaseClass)
                       .Replace(Tokens.OutputSuffix, Config.OutputSuffix);

            e.Dependency.Contents = replaceTokens(e.Dependency.Contents);
            e.Dependency.ObjectName = replaceTokens(e.Dependency.ObjectName);
            e.Dependency.PathFragment = replaceTokens(e.Dependency.PathFragment);

            return e.Dependency;
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            // don't write out object base classes since it's redundant
            var baseTypeString = (objectGraph.BaseType == typeof(object))
                ? string.Empty
                : ": " + objectGraph.BaseTypeFormattedName;

            var output = Template.Raw
                            .Replace(Tokens.Namespace, Config.Namespace)
                            .Replace(Tokens.ObjectName, objectGraph.Name.ToSingular())
                            .Replace(Tokens.PropertiesStub, GetPropertiesReplacement(objectGraph.Properties))
                            .Replace(Tokens.ConstructionStub, GetConstructionReplacement(objectGraph.Properties))
                            .Replace(Tokens.ObjectBaseClass, Config.ObjectBaseClass)
                            .Replace(Tokens.OutputSuffix, Config.OutputSuffix)
                            .Replace(Tokens.BaseTypeName, baseTypeString);

            var outPath = Path.Combine(Config.OutputPath, objectGraph.Name.ToSingular() + ".cs");

            //TODO: Handle generics better while writing out .cs pocos

            File.WriteAllText(outPath.Replace('<', '_').Replace('>', '_'), output); //hacky, can't save fileNames with '<' or '>' in the name

            Text.White($"Wrote '"); Text.Yellow(outPath); Text.WhiteLine("'");

            await Task.CompletedTask;
        }

        private static string GetPropertiesReplacement(IEnumerable<PropertyGraph> properties) //TODO: Figure out something for more configuration of the generators
        {
            string template =
                "\t\t/// <summary>" + Environment.NewLine +
                "\t\t/// Gets or sets the ~PROPERTY_NAME~ property." + Environment.NewLine +
                "\t\t/// <summary>" + Environment.NewLine +
                "\t\tpublic virtual ~PROPERTY_DATATYPE~ ~PROPERTY_NAME~ { get; set; }";

            var sb = new StringBuilder();

            foreach (var p in properties)
            {
                if (p.SourceType.Equals("sysname", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (!p.SourceType.Equals(GenesisDefaults.UnknownSourceType, StringComparison.Ordinal))
                    sb.AppendLine(template.Replace(Tokens.PropertyDataType, p.SourceType.ToCodeDataType())
                        .Replace(Tokens.PropertyName, p.Name)
                        .Replace(Tokens.PropertyMemberName, p.Name.ToCorrectedCase()));
            }

            return sb.ToString();
        }

        private static string GetConstructionReplacement(List<PropertyGraph> properties)
        {
            const string template = "            //this.~PROPERTY_MEMBER_NAME~ = default;";

            var sb = new StringBuilder();

            foreach (var p in properties)
            {
                if (p.SourceType.Equals("sysname", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                sb.AppendLine(template.Replace(Tokens.PropertyMemberName, p.Name.ToCorrectedCase())
                                      .Replace(Tokens.PropertyDataType, p.SourceType.ToCodeDataType()));
            }

            return string.Empty;
        }
    }
}
