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
        public override string CommandText => "aspnet-svc";
        public override string Description => "Typical? ASP.Net service layer boilerplate";
        public override string FriendlyName => "Asp.Net 2nd-layer Services";

        public AspNetServiceConfig Config { get; set; } = new AspNetServiceConfig();

        protected override void OnInitialized()
        {
            Config = (AspNetServiceConfig)Configuration;
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
            var baseTypeString = Config.GenericBaseClass
                                    ? Config.ModelBaseClass + "<TKey>"
                                    : Config.ModelBaseClass;

            string replaceTokens(string input)
                => input.Replace(Tokens.Namespace, Config.DepsNamespace)
                        .Replace(Tokens.OutputSuffix, Config.OutputSuffix)
                        .Replace(Tokens.ObjectBaseClass, baseTypeString)
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
                            .Replace(Tokens.ObjectNameAsArgument, objectGraph.Name.ToCorrectedCase())
                            .Replace(Tokens.KeyDataType, objectGraph.KeyDataType)
                            .Replace(Tokens.OutputSuffix, Config.OutputSuffix)
                            .Replace(Tokens.RepoSuffix, Config.RepoSuffix)
                            .Replace(Tokens.CachedRepoSuffix, Config.CachedRepoSuffix)
                            .Replace(Tokens.DtoSuffix, Config.DtoSuffix)
                            .Replace(Tokens.MapperSuffix, Config.MapperSuffix)
                            .Replace(Tokens.DepsNamespace, Config.DepsNamespace)
                            .Replace(Tokens.DepsDtoNamespace, Config.DtoNamespace)
                            .Replace(Tokens.DepsModelNamespace, Config.ModelNamespace)
                            .Replace(Tokens.DepsMappingNamespace, Config.MapperNamespace)
                            .Replace(Tokens.DepsRepoNamespace, Config.RepoNamespace)
                            .Replace(Tokens.DepsCachedRepoNamespace, Config.CachedRepoNamespace)
                            ;

            var outPath = Path.Combine(Config.OutputPath, objectGraph.Name.ToSingular() + Config.OutputSuffix + ".cs");

            File.WriteAllText(outPath.Replace('<', '_').Replace('>', '_'), output); //HACK: Can't save fileNames with '<' or '>' in the name

            Text.White($"Wrote '"); Text.Yellow(objectGraph.Name.ToSingular() + Config.OutputSuffix + ".cs"); Text.WhiteLine("'"); //TODO: This needs centralized somehow... more copy/paste code

            await Task.CompletedTask;
        }
    }
}
