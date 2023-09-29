using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genesis.Cli;
using Genesis.Output;

#nullable enable 

namespace Genesis.Output.Poco
{
    public class ProjectFilesGenerator : OutputExecutor
    {
        public override string CommandText => "project-files";
        public override string Description => "Generates a project file for each entry in the .gen file";
        public override string FriendlyName => "Project File generator";

        public ProjectFilesConfig Config { get; set; } = new ProjectFilesConfig();

        protected override void OnInitialized()
        {
            Config = (ProjectFilesConfig)Configuration;

            Actions.Add(Tokens.Namespace, (ctx, obj) => Config.Namespace);
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var output = Template.Raw;

            foreach (var proj in Config.Projects)
            {
                var outPath = Path.Combine(proj.Value, proj.Key);

                if (Config.Overwrite == "true")
                {
                    await File.WriteAllTextAsync(outPath.Replace('<', '_').Replace('>', '_'), Template.Raw); //hacky, can't save fileNames with '<' or '>' in the name
                    Text.White($"Wrote '"); Text.Yellow(outPath); Text.WhiteLine("'");
                }
                else
                {
                    Text.White($"Skipped writing '"); Text.Yellow(outPath); Text.WhiteLine("' | overwrite = false");
                }
            }

            var result = new OutputGenesisExecutionResult
            {
                Success = true
            };

            return result;
        }

        protected override GenesisDependency OnBeforeWriteDependency(object sender, DependencyEventArgs e)
        {
            return new GenesisDependency("","",""); //for now
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            await Task.CompletedTask;
        }
    }
}
