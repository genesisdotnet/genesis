using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.Protos
{
    public class ProtoConfig : GeneratorConfiguration
    {
        public int Version { get; set; } = 3;
        public string Overwrite { get; set; } = "all";
        public string Preserve { get; set; } = "none";
    }
}
