using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output
{
    public class OutputGenesisExecutionResult : BlankGenesisExecutionResult, IGenesisExecutionResult
    {
        public override bool Success { get; set; } = true;
        public override string Message { get; set; } = "Success"; //cli warns for no reason apparently
    }
}
