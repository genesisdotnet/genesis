using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    public class BlankTaskResult : ITaskResult
    {
        public BlankTaskResult()
        {

        }

        public virtual bool Success { get; set; } 
        public virtual string Message { get; set; } = string.Empty;
    }
}
