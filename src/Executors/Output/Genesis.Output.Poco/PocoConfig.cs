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
        public string Language { get; set; } = "C#";
        //public bool Overwrite { get; set; } = true;
        //public bool Preserve { get; set; } = true;
    }
}
