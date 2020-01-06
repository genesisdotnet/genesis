using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.Poco
{
    public class JavascriptObjectConfig : GeneratorConfiguration
    {
        public string Language { get; set; } = "Javascript";
        public string Overwrite { get; set; } = "all";
        public string Preserve { get; set; } = "none";
    }
}
