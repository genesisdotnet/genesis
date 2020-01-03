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

            var path = !string.IsNullOrEmpty(Config.DepsPath) && Directory.Exists(Config.DepsPath)
                            ? Config.DepsPath
                            : Config.OutputPath;

            await DepositDependencies(path);

            foreach (var obj in genesis.Objects)
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
                            .Replace(Tokens.OutputSuffix, Config.OutputSuffix)
                            .Replace(Tokens.Injections, Config.Injections.ToInjectionString().TrimEnd(','))
                            .Replace(Tokens.InjectionMembers, Config.Injections.ToInjectionMembersString().TrimEnd(','))
                            .Replace(Tokens.InjectionAssignment, Config.Injections.ToInjectionAssignmentsString().TrimEnd(','))
                            .Replace(Tokens.ApiServiceNamespace, Config.ApiServiceNamespace)
                            .Replace(Tokens.ApiServiceSuffix, Config.ApiServiceSuffix)
                            .Replace(Tokens.InjectionAssignment, Config.Injections.ToInjectionAssignmentsString().TrimEnd(','))
                            .Replace(Tokens.DepsNamespace, Config.DepsNamespace)
                            .Replace(Tokens.DepsDtoNamespace, Config.DepsDtoNamespace)
                            ;

            var path = Path.Combine(Config.OutputPath, $@"{entityName}{Config.OutputSuffix}.cs");

            Text.White($"Wrote '"); Text.Yellow(objectGraph.Name.ToSingular() + Config.OutputSuffix + ".cs"); Text.WhiteLine("'");

            File.WriteAllText(path, output);

            return Task.CompletedTask;
        }
    }
}
