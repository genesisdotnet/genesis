using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output
{
    public class OutputGenesisExecutionResult : BlankGenesisExecutionResult
    {
        public override bool Success { get; set; } = false;
        public override string Message { get; set; } = "Fail";
    }
}
