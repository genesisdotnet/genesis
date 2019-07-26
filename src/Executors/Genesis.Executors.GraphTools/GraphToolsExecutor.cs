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
    public class GraphToolsExecutor : GeneralExecutor
    {
        public override string CommandText => "tools";
        public override string Description => "Tools and utilities for ObjectGraph interaction";
        public override string FriendlyName => "ObjectGraph Tools & Utilities";

        public GraphToolsConfig Config { get; set; } = new GraphToolsConfig();

        protected override void OnInitilized()
        {
            Config = (GraphToolsConfig)Configuration;
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            if(args.Length == 2)
            {
                WriteUsage();
                return await Task.FromResult(new BlankGenesisExecutionResult());
            }

            _ = args[2].ToLower() switch
            {
                "--dump" => WriteObjectGraphToStorage(genesis),
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

        private bool WriteObjectGraphToStorage(GenesisContext genesis)
        {
            var s = new XmlSerializerFactory().CreateSerializer(typeof(List<ObjectGraph>));

            var outDir = Path.Combine(Environment.CurrentDirectory, @"Output");
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            var d = DateTime.UtcNow;
            var fileName = $"ObjectGraphDump_{d.Day}{ d.Hour}{d.Minute}{d.Second}{d.Millisecond}.xml";
            var outputFilePath = Path.Combine(outDir, fileName);

            using var stream = File.OpenWrite(outputFilePath);

            s?.Serialize(stream, genesis.Objects);

            Text.YellowLine($"ObjectGraph written to [{outputFilePath}]");
            return true;
        }
    }
}