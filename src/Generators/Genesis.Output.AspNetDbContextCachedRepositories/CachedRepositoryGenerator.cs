using Genesis.Cli;
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

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string args)
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
            await Task.CompletedTask;

            //Template.Raw.Replace
        }
    }
}
