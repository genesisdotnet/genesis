using Genesis.Generation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.Poco
{
    public class PocoConfig : GeneratorConfiguration
    {
        public string Language { get; set; } = "C#";
    }
}
