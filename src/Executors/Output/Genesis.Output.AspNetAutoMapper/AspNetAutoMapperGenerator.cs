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

namespace Genesis.Output.ApiServiceConfig
{
    public class AspNetAutoMapperGenerator : OutputExecutor
    {
        public override string CommandText => "aspnet-map";
        public override string Description => "AutoMapper mapping profile";
        public override string FriendlyName => "Mapper to convert DTOs <-> DbContext Models";

        public AspNetAutoMapperConfig Config { get; set; } = new AspNetAutoMapperConfig();

        private List<string> _lines = new List<string>();

        protected override void OnInitialized()
        {
            Config = (AspNetAutoMapperConfig)Configuration;            
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputGenesisExecutionResult();

            if (!Directory.Exists(Config.OutputPath)) //TODO: Worry about the output path in the OutputGenerator base
                Directory.CreateDirectory(Config.OutputPath);

            foreach (var obj in genesis.Objects)
                await ExecuteGraph(obj);

            var outPath = Path.Combine(Config.OutputPath, "Mapper.cs");

            var output = Template.Raw
                            .Replace("~MAPPING_CODE~", string.Join(Environment.NewLine, _lines))
                            .Replace(Tokens.DepsModelNamespace, Config.ModelNamespace)
                            .Replace(Tokens.DepsDtoNamespace, Config.DtoNamespace)
                            .Replace(Tokens.DepsNamespace, Config.DepsNamespace)
                            .Replace(Tokens.Namespace, Config.Namespace);

            File.WriteAllText(outPath, output);

            result.Success = true;

            return result;
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            _lines.Add($"\t\t\tCreateMap<{objectGraph.Name.ToSingular()}, {objectGraph.Name.ToSingular()}{Config.DtoBaseClass}>();");

            Text.White($"Appended '"); Text.Yellow(objectGraph.Name.ToSingular() + Config.MapperSuffix); Text.WhiteLine("' content to AutoMapper class.");

            await Task.CompletedTask;
        }
    }
}
