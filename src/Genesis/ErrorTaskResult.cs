using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    public class ErrorGenesisExecutionResult : IGenesisExecutionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
