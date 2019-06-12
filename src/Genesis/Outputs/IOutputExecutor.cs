using Genesis;
using Genesis.Output;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Genesis.Output
{
    public interface IOutputExecutor : IGenesisExecutor<ITaskResult>
    {
        Task AttachDependencies(IList<IOutputDependency> deps);
        Task<bool> DepositDependencies(string outputRoot);

        IGeneratorTemplate Template { get; set; }

        IOutputConfiguration Configuration { get; set; }
        IList<IOutputDependency> Dependencies { get; }
    }
}
