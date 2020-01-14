using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.Poco
{
    /*
     {
        "language": "C#",
        "namespace": "Genesis.Output.Poco", --baseconfig
        "outputPath": "C:\\Temp\\Output", --baseconfig
        "overwrite": "all",
        "preserve": "none"
    }
     * */
    public class PocoConfig : GeneratorConfiguration
    {
        public string ObjectBaseClass { get; set; } = "Model";
        public bool GenericBaseClass { get; set; } = false;
        public string Language { get; set; } = "C#";
        public string Overwrite { get; set; } = "all";
        public string Preserve { get; set; } = "none";
    }
}
