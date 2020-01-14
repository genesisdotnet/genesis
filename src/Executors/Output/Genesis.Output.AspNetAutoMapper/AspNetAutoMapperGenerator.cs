using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genesis.Cli;
using Genesis.Output;

#nullable enable 

namespace Genesis.Output.ApiServiceConfig
{
    public class AspNetServiceGenerator : OutputExecutor
    {
        public override string CommandText => "aspnet-map";
        public override string Description => "AutoMapper mapping profile";
        public override string FriendlyName => "Mapper to convert DTOs <-> DbContext Models";

        public AspNetAutoMapperConfig Config { get; set; } = new AspNetAutoMapperConfig();

        protected override void OnInitialized()
        {
            Config = (AspNetAutoMapperConfig)Configuration;
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputGenesisExecutionResult();

            if (!Directory.Exists(Config.OutputPath)) //TODO: Worry about the output path in the OutputGenerator base
                Directory.CreateDirectory(Config.OutputPath);

            var path = !string.IsNullOrEmpty(Config.DepsPath) && Directory.Exists(Config.DepsPath)
                            ? Config.DepsPath
                            : Config.OutputPath; //TODO: This is still kinda ghetto; feels unprepared

            await DepositDependencies(path);

            foreach (var obj in genesis.Objects)
                await ExecuteGraph(obj);

            result.Success = true;

            return result;
        }

        protected override GenesisDependency OnBeforeWriteDependency(object sender, DependencyEventArgs e)
        {
            var modelBaseType = Config.GenericBaseClass
                                    ? Config.ModelBaseClass + "<TKey>"
                                    : Config.ModelBaseClass;
            
            var dtoBaseType = Config.GenericBaseClass
                                    ? Config.DtoBaseClass + "<TKey>"
                                    : Config.DtoBaseClass;

            string replaceTokens(string input)
                => input.Replace(Tokens.Namespace, Config.DepsNamespace)
                        .Replace(Tokens.OutputSuffix, Config.OutputSuffix)
                        .Replace(Tokens.ObjectBaseClass, modelBaseType)
                        .Replace(Tokens.DtoBaseClass, dtoBaseType)
                        .Replace(Tokens.DepsNamespace, Config.DepsNamespace)
                        .Replace(Tokens.DepsModelNamespace, Config.ModelNamespace);  //NOTE: its //2020: and this still isn't static and somehow additive
                                                                                         //2020: still using string.Replace().Replace()...
            e.Dependency.Contents = replaceTokens(e.Dependency.Contents);
            e.Dependency.ObjectName = replaceTokens(e.Dependency.ObjectName);
            e.Dependency.PathFragment = replaceTokens(e.Dependency.PathFragment);

            return e.Dependency;
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            var output = Template.Raw
                            .Replace(Tokens.Namespace, Config.Namespace)                    //NOTE: see //2020:
                            .Replace(Tokens.ObjectName, objectGraph.Name.ToSingular())
                            .Replace(Tokens.OutputSuffix, Config.OutputSuffix)
                            .Replace(Tokens.DtoBaseClass, Config.DtoBaseClass)
                            .Replace(Tokens.RepoSuffix, Config.RepoSuffix)
                            .Replace(Tokens.CachedRepoSuffix, Config.CachedRepoSuffix)
                            .Replace(Tokens.MapperSuffix, Config.MapperSuffix)
                            .Replace(Tokens.DepsNamespace, Config.DepsNamespace)
                            .Replace(Tokens.DepsDtoNamespace, Config.DtoNamespace)
                            .Replace(Tokens.DepsModelNamespace, Config.ModelNamespace)
                            .Replace(Tokens.DepsMappingNamespace, Config.MapperNamespace)
                            .Replace(Tokens.DepsRepoNamespace, Config.RepoNamespace)
                            .Replace(Tokens.DepsCachedRepoNamespace, Config.CachedRepoNamespace)
                            ;

            var outPath = Path.Combine(Config.OutputPath, objectGraph.Name.ToSingular() + Config.MapperSuffix + ".cs");

            File.WriteAllText(outPath.Replace('<', '_').Replace('>', '_'), output); //HACK: Can't save fileNames with '<' or '>' in the name

            Text.White($"Wrote '"); Text.Yellow(outPath); Text.WhiteLine("'");

            await Task.CompletedTask;
        }
    }
}
