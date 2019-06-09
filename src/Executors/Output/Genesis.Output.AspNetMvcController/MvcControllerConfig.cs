using Genesis.Generation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.MvcController
{
    public class MvcControllerConfig : GeneratorConfiguration
    {
        public string Language { get; set; } = "C#";
    }
}
