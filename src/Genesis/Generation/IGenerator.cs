using Genesis.Cli;
using Genesis.Generation;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Genesis.Generation
{
    public interface IGenerator : ITaskExecutor<ITaskResult>
    {
        IGeneratorTemplate Template { get; set; }

        IGeneratorConfiguration Configuration { get; set; }
    }
}
