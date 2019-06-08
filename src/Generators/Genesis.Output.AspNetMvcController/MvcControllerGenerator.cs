using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genesis;
using Genesis.Generation;

namespace Genesis.Output.MvcController
{
    public class MvcControllerGenerator : Generator
    {
        public override string CommandText => "aspnet-mvc-con";
        public override string Description => "Generates an Asp.Net Controller based on an entity.";
        public override string FriendlyName => "Asp.Net MvcController Generator";

        public MvcControllerConfig Config { get; set; } 

        protected override void OnInitilized()
        {
            Config = (MvcControllerConfig)Configuration;
        }

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputTaskResult(); //overridden just to loop over all the graphs

            foreach(var obj in genesis.Objects)
            {
                await ExecuteGraph(obj);
            }

            return result;
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            var tmp = this.Template;

            if (Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);

            Thread.Sleep(100); //below fails saying directory doesn't exist... even though ^ runs fine?!

            //TODO: Output types are still sourcetypes, not poco types

            var output = tmp.Raw.Replace(Tokens.Namespace, Config.Namespace)    //TODO: Templating engine? / razor etc would be cool ..|., T4 
                                .Replace(Tokens.ObjectName, objectGraph.Name.ToSingular());

            var subPath = Path.Combine(OutputPath, "MvcControllers");

            if (!Directory.Exists(subPath))
                Directory.CreateDirectory(subPath);

            await Task.Delay(100); //timing or something weirdness

            File.WriteAllText(Path.Combine(subPath, objectGraph.Name.ToSingular() + "Controller.cs"), output);            
        }
    }
}
