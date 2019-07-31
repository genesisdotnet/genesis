using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genesis.Cli;
using Genesis.Output;

namespace Genesis.Output.Poco
{
    public class PocoGenerator : OutputExecutor
    {
        public override string CommandText => "poco";
        public override string Description => "(P)lain (O)ld (C)Sharp (O)bject .cs file";
        public override string FriendlyName => "(P)lain (O)ld (C)Sharp (O)bject";

        public PocoConfig Config { get; set; }

        protected override void OnInitialized()
        {
            Config = (PocoConfig)Configuration;
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputGenesisExecutionResult();

            try
            {
                foreach (var obj in genesis.Objects)
                {
                    await ExecuteGraph(obj);
                }
                result.Success = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                result.Message = e.Message;
            }
            
            return result;
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            var tmp = Template;

            if (!Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);

            // don't write out object base classes since it's redundant
            var baseTypeString = (objectGraph.BaseType == typeof(object))
                ? string.Empty
                : ": " + objectGraph.BaseTypeFormattedName;

            var output = tmp.Raw
                .Replace(Tokens.Namespace,Config.Namespace) //TODO: Templating engine? / razor etc would be cool ..|., T4 
                .Replace(Tokens.ObjectName, objectGraph.Name.ToSingular())
                .Replace(Tokens.PropertiesStub, GetPropertiesReplacement(objectGraph.Properties))
                .Replace(Tokens.ConstructionStub, GetConstructionReplacement(objectGraph.Properties))
                .Replace(Tokens.BaseTypeName, baseTypeString)
                ; 

            var subPath = Path.Combine(OutputPath, "Pocos");

            if (!Directory.Exists(subPath))
                Directory.CreateDirectory(subPath);

            var outPath = Path.Combine(subPath, objectGraph.Name.ToSingular() + ".cs");

            //TODO: Handle generics better while writing out .cs pocos

            File.WriteAllText(outPath.Replace('<', '_').Replace('>', '_'), output); //hacky, can't save fileNames with '<' or '>' in the name

            Text.White($"Wrote '"); Text.Yellow(objectGraph.Name.ToSingular() + ".cs"); Text.WhiteLine("'");

            await Task.CompletedTask;
        }

        private string GetPropertiesReplacement(IEnumerable<PropertyGraph> properties) //TODO: Figure out something for more configuration of the generators
        {
            string template = "\t\tprivate ~PROPERTY_DATATYPE~ ~PROPERTY_MEMBER_NAME~;" + Environment.NewLine +
                                "\t\tpublic ~PROPERTY_DATATYPE~ ~PROPERTY_NAME~" + Environment.NewLine +
                                "\t\t{" + Environment.NewLine +
                                "\t\t\tget => ~PROPERTY_MEMBER_NAME~;" + Environment.NewLine +
                                "\t\t\tset => ~PROPERTY_MEMBER_NAME~;" + Environment.NewLine +
                                "\t\t}" + Environment.NewLine;

            var sb = new StringBuilder();

            foreach (var p in properties)
            {
                if (!p.SourceType.Equals(GenesisDefaults.UnknownSourceType, StringComparison.Ordinal))
                    sb.AppendLine(template.Replace(Tokens.PropertyDataType, p.SourceType.ToCodeDataType())
                        .Replace(Tokens.PropertyName, p.Name)
                        .Replace(Tokens.PropertyMemberName, p.Name.ToCorrectedCase()));
            }

            return sb.ToString();
        }

        private string GetConstructionReplacement(List<PropertyGraph> properties)
        {
            const string template = "            //this.~PROPERTY_MEMBER_NAME~ = default(~PROPERTY_DATATYPE~);";

            var sb = new StringBuilder();

            foreach (var p in properties)
            {
                if (p.SourceType == "sysname")
                    continue;

                sb.AppendLine(template.Replace(Tokens.PropertyMemberName, p.Name.ToCorrectedCase())
                                    .Replace(Tokens.PropertyDataType, p.SourceType.ToCodeDataType()));
            }

            return sb.ToString();
        }
    }
}
