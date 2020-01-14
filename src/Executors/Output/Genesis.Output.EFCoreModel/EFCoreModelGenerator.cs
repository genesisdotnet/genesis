using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genesis.Cli;
using Genesis.Output;

#nullable enable 

namespace Genesis.Output.EFCoreModel
{
    public class EFCoreModelGenerator : OutputExecutor
    {
        public override string CommandText => "ef-model";
        public override string Description => "Entity Framework Core Model generator";
        public override string FriendlyName => "EFCore Models";

        public EFCoreModelConfig Config { get; set; } = new EFCoreModelConfig();

        protected override void OnInitialized()
        {
            Config = (EFCoreModelConfig)Configuration;
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputGenesisExecutionResult();
                
            if (!Directory.Exists(Config.OutputPath)) //TODO: Worry about the output path in the OutputGenerator base
                    Directory.CreateDirectory(Config.OutputPath);

            var path = !string.IsNullOrEmpty(Config.DepsPath) && Directory.Exists(Config.DepsPath)
                            ? Config.DepsPath
                            : Config.OutputPath;

            await DepositDependencies(path);

            foreach (var obj in genesis.Objects)
                await ExecuteGraph(obj);

            result.Success = true;
            
            return result;
        }

        protected override GenesisDependency OnBeforeWriteDependency(object sender, DependencyEventArgs e)
        {
             string replaceTokens(string input)
                => input.Replace(Tokens.Namespace, Config.DepsNamespace)  //TODO: Make more base level config properties so this can be global-er
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
            var baseTypeString = Config.GenericBaseClass
                                    ? Config.ObjectBaseClass+'<'+ objectGraph.KeyDataType.ToCodeDataType()+'>'
                                    : Config.ObjectBaseClass;

            var output = Template.Raw
                            .Replace(Tokens.Namespace, Config.Namespace)
                            .Replace(Tokens.ObjectName, objectGraph.Name.ToSingular())
                            .Replace(Tokens.PropertiesStub, GetPropertiesReplacement(objectGraph.Properties))
                            .Replace(Tokens.ConstructionStub, GetConstructionReplacement(objectGraph.Properties))
                            .Replace(Tokens.RelationshipStub, GetRelationshipsReplacement(objectGraph))
                            .Replace(Tokens.OutputSuffix, Config.OutputSuffix)
                            .Replace(Tokens.BaseTypeName, baseTypeString)
                            .Replace(Tokens.DepsNamespace, Config.DepsNamespace);

            var outPath = Path.Combine(Config.OutputPath, objectGraph.Name.ToSingular() + ".cs");

            //TODO: Handle generics better while writing out .cs pocos

            File.WriteAllText(outPath.Replace('<', '_').Replace('>', '_'), output); //hacky, can't save fileNames with '<' or '>' in the name

            Text.White($"Wrote '"); Text.Yellow(outPath); Text.WhiteLine("'");

            await Task.CompletedTask;
        }

        private static string GetRelationshipsReplacement(ObjectGraph og)
        {
            var PTS = "~PRIMARY_TABLE_SINGLE~";
            //var PTC = "~PRIMARY_TABLE_PLURAL~";
            var FTP = "~FOREIGN_TABLE_PLURAL~";
            var FTS = "~FOREIGN_TABLE_SINGLE~";
            var FTC = "~FOREIGN_TABLE_COLUMN~";

            var navTemplate =
                $"\t\tpublic virtual {PTS} {FTC} {{ get; set; }} = new {PTS}();";
            
            var pkTemplate = 
                $"\t\tpublic virtual ICollection<{FTS}> {FTC}{FTP} {{ get; set; }} = new HashSet<{FTS}>();";
            
            var sb = new StringBuilder();

            og.Relationships
                .Where(x=>x.ForeignTable == og.Name)
                .ToList()
                .ForEach(r => {
                    sb.AppendLine(navTemplate
                                    .Replace(PTS, r.PrimaryTable.ToSingular())
                                    .Replace(FTC, r.ForeignColumn.TrimEnd('i', 'I', 'd', 'D'))
                    );
                });

            og.Relationships
                .Where(x => x.PrimaryTable == og.Name)
                .ToList()
                .ForEach(r => {
                    sb.AppendLine(pkTemplate
                                    .Replace(FTS, r.ForeignTable.ToSingular())
                                    .Replace(FTC, r.ForeignColumn.TrimEnd('i', 'I', 'd', 'D'))
                                    .Replace(FTP, r.ForeignTable)
                    );
                });

            return sb.ToString();
        }

        private static string GetPropertiesReplacement(IEnumerable<PropertyGraph> properties) //TODO: Figure out something for more configuration of the generators
        {
            string template = 
                "\t\t/// <summary>" + Environment.NewLine +
                "\t\t/// Gets or sets the ~PROPERTY_NAME~." + Environment.NewLine + 
                "\t\t/// <summary>" + Environment.NewLine +
                "\t\tpublic ~PROPERTY_DATATYPE~ ~PROPERTY_NAME~ { get; set; }~INIT~";
                    
            var sb = new StringBuilder();

            foreach (var p in properties)
            {
                //TODO: Make this configurable to skip the Key

                if (p.Name.Equals("Id", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (p.SourceType.Equals("sysname", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (!p.SourceType.Equals(GenesisDefaults.UnknownSourceType, StringComparison.Ordinal))
                    sb.AppendLine(template.Replace(Tokens.PropertyDataType, p.SourceType.ToCodeDataType())
                        .Replace(Tokens.PropertyName, p.Name)
                        .Replace(Tokens.PropertyMemberName, p.Name.ToCorrectedCase()))
                        .Replace("~INIT~", p.SourceType.ToCodeDataType().Equals("string", StringComparison.OrdinalIgnoreCase)
                                                ? " = string.Empty;"
                                                : string.Empty);
            }

            return sb.ToString();
        }

        private static string GetConstructionReplacement(List<PropertyGraph> properties)
        {
            const string template = "            //this.~PROPERTY_MEMBER_NAME~ = default;";
            //TODO: Initialize model members from EFCore model generator
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
