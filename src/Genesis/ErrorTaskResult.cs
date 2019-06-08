using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    public class ErrorTaskResult : ITaskResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
