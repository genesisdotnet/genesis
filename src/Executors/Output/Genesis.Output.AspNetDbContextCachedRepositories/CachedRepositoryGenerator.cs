using Genesis;
using Genesis.Generation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Output.CachedRepo
{
    public class CachedRepositoryGenerator : Generator
    {
        public override string CommandText => "aspnet-repo-cached";
        public override string Description => "Implements a simple cached repository";
        public override string FriendlyName => "Asp.Net Cached Repository";

        public CachedRepoConfig Config { get; set; }

        protected override void OnInitilized()
        {
            Config = (CachedRepoConfig)Configuration;
        }

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputTaskResult();

            foreach (var obj in genesis.Objects)
            {
                await ExecuteGraph(obj);
            }

            return result;
        }

        private async Task ExecuteGraph(ObjectGraph objGraph)
        {
            var output = Template.Raw.Replace(Tokens.Namespace, Config.Namespace) 
                                     .Replace(Tokens.ObjectName, objGraph.Name.ToSingular());

            var subPath = Path.Combine(OutputPath, "Pocos");

            if (!Directory.Exists(subPath))
                Directory.CreateDirectory(subPath);

            File.WriteAllText(Path.Combine(subPath, $"Cached{objGraph.Name.ToSingular()}Repository.cs"), output);

            await Task.CompletedTask;
        }
    }
}
