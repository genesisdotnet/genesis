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
        public virtual string Message { get; set; } = "Blank... empty. Void of contents, if there were any at all...";
    }
}
