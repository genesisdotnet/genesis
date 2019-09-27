using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

#nullable enable

namespace Genesis.Output.AspNetMvcController
{
    public class MvcControllerGenerator : OutputExecutor
    {
        public override string CommandText => "aspnet-mvc-con";
        public override string Description => "Generates an Asp.Net Controller based on an entity.";
        public override string FriendlyName => "Asp.Net MvcController";

        public MvcControllerConfig Config { get; set; } = new MvcControllerConfig();

        protected override void OnInitialized()
        {
            Config = (MvcControllerConfig)Configuration;
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            Text.DarkGrayLine($@"Generating MVC Controllers in: {Config.OutputPath}");
            
            foreach(var obj in genesis.Objects)
                await ExecuteGraph(obj);
            
            return new OutputGenesisExecutionResult();
        }

        public Task ExecuteGraph(ObjectGraph objectGraph)
        {
            if (Config.OutputPath.EndsWith("/") || Config.OutputPath.EndsWith(@"\"))
                Config.OutputPath = Config.OutputPath[..^1];

            if (!Directory.Exists(Config.OutputPath))
                Directory.CreateDirectory(Config.OutputPath); 
            
            var entityName = objectGraph.Name.ToSingular();

            var output = Template.Raw
                            .Replace(Tokens.Namespace, Config.Namespace)
                            .Replace(Tokens.ObjectName, entityName)
                            .Replace(Tokens.Injections, Config.Injections.ToInjectionString())
                            .Replace(Tokens.InjectionMembers, Config.Injections.ToInjectionMembersString())
                            .Replace(Tokens.InjectionAssignment, Config.Injections.ToInjectionAssignmentsString());
            
            Text.DarkCyanLine($@"{Config.OutputPath}/{entityName}Controller.cs");

            File.WriteAllText(Path.Combine(Config.OutputPath, $"{entityName}Controller.cs"), output);

            return Task.CompletedTask;
        }
    }
}
