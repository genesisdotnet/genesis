using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.EFCoreModel
{
    public class EFCoreModelConfig : GeneratorConfiguration
    {
        public string ObjectBaseClass { get; set; } = "Model";
        public string Language { get; set; } = "C#";
        public string Overwrite { get; set; } = "all";
        public string Preserve { get; set; } = "none";
        public bool GenericBaseClass { get; set; } = true;
    }
}
