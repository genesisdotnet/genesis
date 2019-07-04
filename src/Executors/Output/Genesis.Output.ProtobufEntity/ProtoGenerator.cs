using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Output.Protos
{
    public class ProtoGenerator : OutputExecutor
    {
        public override string CommandText => "proto-entity";
        public override string Description => "Generates a Protobuf file with CRUD operations and an Entity member";
        public override string FriendlyName => "Protobuf (.proto) file Generator";

        public ProtoConfig Config { get; set; }

        protected override void OnInitilized()
        {
            Config = (ProtoConfig)Configuration;
        }

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputTaskResult();

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

            var output = tmp.Raw.Replace(Tokens.Version, Config.Version.ToString())
                                .Replace(Tokens.PropertiesStub, GetPropertiesReplacement(objectGraph.Properties))
                                .Replace(Tokens.ObjectName, objectGraph.Name.ToSingular())
                                ;

            var subPath = Path.Combine(OutputPath, "Protos");

            if (!Directory.Exists(subPath))
                Directory.CreateDirectory(subPath);

            var outPath = Path.Combine(subPath, objectGraph.Name.ToSingular() + ".proto");

            File.WriteAllText(outPath, output);

            Text.White($"Wrote '"); Text.Yellow(objectGraph.Name.ToSingular() + ".proto"); Text.WhiteLine("'");
            Text.Line();

            await Task.CompletedTask;
        }

        private string GetPropertiesReplacement(List<PropertyGraph> properties)
        {
            string template = "\t\t~PROPERTY_DATATYPE~ ~PROPERTY_MEMBER_NAME~ = ~COUNTER~" + Environment.NewLine;

            var sb = new StringBuilder();

            var count = 1;
            foreach (var p in properties)
            {
                if (p.SourceType.ToLower() == "sysname") //TODO: Exclusion list on each input executor? 
                {
                    //TODO: display something in the UI?
                    continue;
                }

                if (!p.SourceType.Equals(GenesisDefaults.UnknownSourceType, StringComparison.Ordinal))
                    sb.AppendLine(template.Replace(Tokens.PropertyDataType, p.SourceType.ToBasicDataType())
                        .Replace(Tokens.PropertyName, p.Name)
                        .Replace(Tokens.PropertyCounter, count++.ToString())
                        .Replace(Tokens.PropertyMemberName, p.Name.ToCorrectedCase()));
            }

            return sb.ToString();
        }
    }
}
