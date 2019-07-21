using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genesis;
using Genesis.Output;

namespace Genesis.Output.MvcController
{
    public class MvcControllerGenerator : OutputExecutor
    {
        public override string CommandText => "aspnet-mvc-con";
        public override string Description => "Generates an Asp.Net Controller based on an entity.";
        public override string FriendlyName => "Asp.Net MvcController";

        public MvcControllerConfig Config { get; set; } 

        protected override void OnInitilized()
        {
            Config = (MvcControllerConfig)Configuration;
        }

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            Text.DarkGrayLine($@"Generating MVC Controllers in: {Config.OutputPath}");
            var result = new OutputTaskResult(); //overridden just to loop over all the graphs

            foreach(var obj in genesis.Objects)
                await ExecuteGraph(obj);
            
            return result;
        }

        public Task ExecuteGraph(ObjectGraph objectGraph)
        {
            var tmp = this.Template;

            if (Directory.Exists(Config.OutputPath))
                Directory.CreateDirectory(OutputPath);

            var entityName = objectGraph.Name.ToSingular();

            var output = tmp.Raw.Replace(Tokens.Namespace, Config.Namespace)    //TODO: Templating engine? / razor etc would be cool ..|., T4 
                                .Replace(Tokens.ObjectName, entityName);

            //TODO: Add logic to handle '/' or lack thereof at the end of Config.OutputPath

            Text.DarkCyanLine($@"{Config.OutputPath}/{entityName}Controller.cs");

            File.WriteAllText(Path.Combine(Config.OutputPath, $"{entityName}Controller.cs"), output);

            return Task.CompletedTask;
        }
    }
}
