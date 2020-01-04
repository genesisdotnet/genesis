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
            if (!Directory.Exists(Config.OutputPath))
                Directory.CreateDirectory(Config.OutputPath);

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
            var entityName = objectGraph.Name.ToSingular();

            var output = Template.Raw
                            .Replace(Tokens.Namespace, Config.Namespace)
                            .Replace(Tokens.ObjectName, entityName)
                            .Replace(Tokens.ObjectNameAsArgument, entityName.ToCorrectedCase())
                            .Replace(Tokens.OutputSuffix, Config.OutputSuffix)
                            .Replace(Tokens.ServiceSuffix, Config.ServiceSuffix)
                            .Replace(Tokens.DtoSuffix, Config.DtoSuffix)
                            .Replace(Tokens.DepsNamespace, Config.DepsNamespace)
                            .Replace(Tokens.DepsDtoNamespace, Config.DepsDtoNamespace)
                            .Replace(Tokens.DepsServiceNamespace, Config.DepsServiceNamespace)
                            ;

            var path = Path.Combine(Config.OutputPath, $@"{entityName}{Config.OutputSuffix}.cs");

            File.WriteAllText(path, output);

            Text.White($"Wrote '"); Text.Yellow(path); Text.WhiteLine("'");

            return Task.CompletedTask;
        }
    }
}
