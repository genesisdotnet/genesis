using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.Poco
{
    public class ProjectFilesConfig : GeneratorConfiguration
    {
        public Dictionary<string, string> Projects { get; set; } = new();
        public string Language { get; set; } = "C#";
        public string Overwrite { get; set; } = "all";
        public string Preserve { get; set; } = "none";
    }
}
