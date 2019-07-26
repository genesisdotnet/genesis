using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Executors
{
    public interface IGeneralExecutor : IGenesisExecutor<IGenesisExecutionResult>
    {
        IGeneralConfiguration Configuration { get; set; }
    }
}
