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
    public class JavascriptObjectGenerator : OutputExecutor
    {
        public override string CommandText => "js-dto";
        public override string Description => "Creates a basic javascript data transfer object with 'typed' properties";
        public override string FriendlyName => "Javascript DTO Generator";

        public JavascriptObjectConfig Config { get; set; } = new JavascriptObjectConfig();

        protected override void OnInitialized()
        {
            Config = (JavascriptObjectConfig)Configuration;
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
                       .Replace(Tokens.OutputSuffix, Config.OutputSuffix);

            e.Dependency.Contents = replaceTokens(e.Dependency.Contents);
            e.Dependency.ObjectName = replaceTokens(e.Dependency.ObjectName);
            e.Dependency.PathFragment = replaceTokens(e.Dependency.PathFragment);

            return e.Dependency;
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            var output = Template.Raw
                            .Replace(Tokens.Namespace, Config.Namespace)
                            .Replace(Tokens.ObjectName, objectGraph.Name.ToSingular())
                            .Replace(Tokens.PropertiesStub, GetPropertiesReplacement(objectGraph.Properties))
                            .Replace(Tokens.OutputSuffix, Config.OutputSuffix);

            var outPath = Path.Combine(Config.OutputPath, objectGraph.Name.ToSingular() + Config.OutputSuffix + ".js");

            File.WriteAllText(outPath, output);

            Text.White($"Wrote '"); Text.Yellow(outPath); Text.WhiteLine("'");

            await Task.CompletedTask;
        }

        private static string GetPropertiesReplacement(IEnumerable<PropertyGraph> properties)
        {
            static string getInitString(string datatype)
                => (datatype) switch
                    {
                        "byte" => "0",
                        "short" => "0",
                        "int" => "0",
                        "long" => "0",
                        "decimal" => "0",
                        "double" => "0",
                        "bool" => "false",
                        "DateTime" => "new Date()",
                        "DateTimeOffset" => "new Date()",
                        _ => "''"
                    };            

            string template =$"\t\tthis.~PROPERTY_MEMBER_NAME~ = options.~PROPERTY_MEMBER_NAME~ || ~PROPERTY_DATATYPE~;";

            var sb = new StringBuilder();

            foreach (var p in properties)
            {
                if (p.SourceType.Equals("sysname", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (!p.SourceType.Equals(GenesisDefaults.UnknownSourceType, StringComparison.Ordinal))
                    sb.AppendLine(
                        template.Replace(Tokens.PropertyMemberName, p.Name.ToCorrectedCase())
                                .Replace(Tokens.PropertyDataType, getInitString(p.SourceType.ToCodeDataType())));
            }

            return sb.ToString();
        }
    }
}
