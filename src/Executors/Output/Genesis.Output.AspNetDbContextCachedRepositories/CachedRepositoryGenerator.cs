﻿using Genesis;
using Genesis.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Output.CachedRepo
{
    public class CachedRepositoryGenerator : OutputExecutor
    {
        public override string CommandText => "aspnet-repo-cached"; //UX: This is annoying to type, though descriptive
        public override string Description => "Implements a simple cached repository";
        public override string FriendlyName => "Asp.Net Cached Repository";

        public CachedRepoConfig Config { get; set; }

        protected override void OnInitialized()
        {
            Config = (CachedRepoConfig)Configuration;
        }

        protected override GenesisDependency OnBeforeWriteDependency(object sender, DependencyEventArgs e)
        {
            var baseTypeString = Config.GenericBaseClass
                        ? Config.ModelBaseClass + "<TKey>"
                        : Config.ModelBaseClass;

            string replaceTokens(string input)
               => input.Replace(Tokens.Namespace, Config.DepsNamespace)  //TODO: Make more base level config properties so this can be global-er
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
                                     ;

            if (!Directory.Exists(Config.OutputPath))
                Directory.CreateDirectory(Config.OutputPath);

            File.WriteAllText(Path.Combine(Config.OutputPath, $"{objGraph.Name.ToSingular()}{Config.OutputSuffix}.cs"), output);

            await Task.CompletedTask;
        }
    }
}
