using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Genesis.Cli;

namespace Genesis.Population
{
    public interface IPopulator : ITaskExecutor<ITaskResult>
    {
        IPopulatorConfiguration Configuration { get; set; }
    }
}
