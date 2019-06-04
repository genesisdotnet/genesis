using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Generation
{
    public class OutputTaskResult : BlankTaskResult
    {
        public override bool Success { get; set; } = false;
        public override string Message { get; set; } = "Fail";
    }
}
