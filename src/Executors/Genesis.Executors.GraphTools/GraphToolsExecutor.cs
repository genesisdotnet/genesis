using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Genesis.Output;

namespace Genesis.Executors.GraphTools
{
    public class GraphToolsExecutor : OutputExecutor
    {
        public override string CommandText => "tools";
        public override string Description => "Tools and utilities for ObjectGraph interaction";
        public override string FriendlyName => "ObjectGraph Tools & Utilities";

        public GraphToolsConfig Config { get; set; }

        protected override void OnInitilized()
        {
            Config = (GraphToolsConfig)Configuration;
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            _ = args[0].ToLower() switch 
            {
                "--dump" => WriteObjectGraphToStorage(genesis, args),
                _ => WriteUsage()
            };
            return await Task.FromResult(new BlankGenesisExecutionResult());
        }

        private static bool WriteUsage()
        {
            Text.YellowLine("Graph Tools Usage");
            Text.WhiteLine();
            Text.WhiteLine("--dump              Writes the ObjectGraphs content to Xml");
            Text.WhiteLine("--clear             Empties the current ObjectGraphs collection");
            Text.WhiteLine();
            return true; // wanted to use the new switch syntax ;P
        }

        private bool WriteObjectGraphToStorage(GenesisContext genesis, string[] args)
        {
            var s = new XmlSerializerFactory().CreateSerializer(typeof(ObjectGraph));
            var outputFilePath = Path.Combine(Config.OutputPath, $"ObjectGraphDump_{DateTime.UtcNow.ToShortDateString() + DateTime.UtcNow.ToShortTimeString()}");
            using var stream = File.OpenWrite(outputFilePath);
            s?.Serialize(stream, genesis.Objects);
            
            Text.YellowLine($"ObjectGraph written to [{outputFilePath}]");
            return true;
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            var tmp = Template;

            if (!Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);

            var output = tmp.Raw.Replace(Tokens.Namespace, Config.Namespace)    //TODO: Templating engine? / razor etc would be cool ..|., T4 
                                .Replace(Tokens.ObjectName, objectGraph.Name.ToSingular())
                                .Replace(Tokens.PropertiesStub, GetPropertiesReplacement(objectGraph.Properties))
                                .Replace(Tokens.ConstructionStub, GetConstructionReplacement(objectGraph.Properties));

            var subPath = Path.Combine(OutputPath, "Pocos");

            if (!Directory.Exists(subPath))
                Directory.CreateDirectory(subPath);

            var outPath = Path.Combine(subPath, objectGraph.Name.ToSingular() + ".cs");

            File.WriteAllText(outPath, output);

            Text.White($"Wrote '"); Text.Yellow(objectGraph.Name.ToSingular() + ".cs"); Text.WhiteLine("'");

            await Task.CompletedTask;
        }

        private string GetPropertiesReplacement(List<PropertyGraph> properties) //TODO: Figure out something for more configuration of the generators
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
