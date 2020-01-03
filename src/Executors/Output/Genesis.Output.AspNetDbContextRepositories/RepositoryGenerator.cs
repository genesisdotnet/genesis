using System.IO;
using System.Threading.Tasks;

namespace Genesis.Output.CachedRepo
{
    public class RepositoryGenerator : OutputExecutor
    {
        public override string CommandText => "aspnet-repo";
        public override string Description => "Implements a simple repository based on EF Core";
        public override string FriendlyName => "Asp.Net DbContext Repository";

        public RepoConfig Config { get; set; }

        protected override void OnInitialized()
        {
            Config = (RepoConfig)Configuration;
        }

        protected override GenesisDependency OnBeforeWriteDependency(object sender, DependencyEventArgs e)
        {
            var baseTypeString = Config.GenericBaseClass
                        ? Config.ModelBaseClass + "<TKey>"
                        : Config.ModelBaseClass;

            string replaceTokens(string input)
               => input.Replace(Tokens.Namespace, Config.DepsNamespace)
                       .Replace(Tokens.ObjectBaseClass, Config.ObjectBaseClass)
                       .Replace(Tokens.OutputSuffix, Config.OutputSuffix)
                       .Replace(Tokens.ObjectBaseClass, baseTypeString)
                       .Replace(Tokens.DepsNamespace, Config.DepsNamespace)
                       .Replace(Tokens.DepsModelNamespace, Config.DepsModelNamespace);

            e.Dependency.Contents = replaceTokens(e.Dependency.Contents);
            e.Dependency.ObjectName = replaceTokens(e.Dependency.ObjectName);
            e.Dependency.PathFragment = replaceTokens(e.Dependency.PathFragment);

            return e.Dependency;
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputGenesisExecutionResult();

            foreach (var obj in genesis.Objects)
            {
                await ExecuteGraph(obj);
            }

            return result;
        }

        private async Task ExecuteGraph(ObjectGraph objGraph)
        {
            var baseTypeString = Config.GenericBaseClass
                        ? Config.ModelBaseClass + '<' + objGraph.KeyDataType + '>'
                        : Config.ModelBaseClass;

            var output = Template.Raw.Replace(Tokens.Namespace, Config.Namespace) 
                                     .Replace(Tokens.ObjectName, objGraph.Name.ToSingular() + Config.OutputSuffix)
                                     .Replace(Tokens.ObjectBaseClass, baseTypeString)
                                     .Replace(Tokens.DepsNamespace, Config.DepsNamespace)
                                     .Replace(Tokens.DepsModelNamespace, Config.DepsModelNamespace)
                                     .Replace(Tokens.KeyDataType, objGraph.KeyDataType)
                                     ;

            if (!Directory.Exists(Config.OutputPath))
                Directory.CreateDirectory(Config.OutputPath);

            File.WriteAllText(Path.Combine(Config.OutputPath, $"{objGraph.Name.ToSingular()}{Config.OutputSuffix}.cs"), output);

            await Task.CompletedTask;
        }
    }
}
