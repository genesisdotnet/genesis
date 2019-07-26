using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    public class BlankGenesisExecutionResult : IGenesisExecutionResult
    {
        public BlankGenesisExecutionResult()
        {

        }

        public virtual bool Success { get; set; } 
        public virtual string Message { get; set; } = string.Empty;
    }
}
