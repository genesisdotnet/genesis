using System.Collections.Generic;

#nullable enable

namespace Genesis.Output.AspNetMvcController
{
    public class MvcControllerConfig : GeneratorConfiguration
    {
        public string Language { get; set; } = "C#";
        public bool Overwrite { get; set; } = true;
        public string Preserve { get; set; } = string.Empty;
        public string ApiServiceNamespace { get; set; } = "Genesis.Services";
        public string ApiServiceSuffix { get; set; } = "Service";
        public bool InitNullable { get; set; } = true;
        public List<string> Injections { get; set; } = new List<string>();
    }
}