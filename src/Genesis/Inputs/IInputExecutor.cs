using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Genesis;

namespace Genesis.Input
{
    public interface IInputExecutor : IGenesisExecutor<IGenesisExecutionResult>
    {
        IInputConfiguration Configuration { get; set; }
    }
}
