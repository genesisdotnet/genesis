using Genesis;
using Genesis.Generation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Genesis.Generation
{
    public interface IGenerator : IGenesisExecutor<ITaskResult>
    {
        Task AttachDependencies(IList<IGenesisDependency> deps);
        Task<bool> DepositDependencies(string outputRoot);

        IGeneratorTemplate Template { get; set; }

        IGeneratorConfiguration Configuration { get; set; }
        IList<IGenesisDependency> Dependencies { get; }
    }
}
