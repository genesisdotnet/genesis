using Genesis;
using Genesis.Output;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Genesis.Output
{
    public interface IOutputExecutor : IGenesisExecutor<IGenesisExecutionResult>
    {
        Task AttachDependencies(IList<IOutputDependency> deps);
        Task<bool> DepositDependencies(string outputRoot);

        IDictionary<string, Func<GenesisContext, ObjectGraph, string>> Actions { get; }

        IGeneratorTemplate Template { get; set; }

        IOutputConfiguration Configuration { get; set; }
        IList<IOutputDependency> Dependencies { get; }
    }
}
