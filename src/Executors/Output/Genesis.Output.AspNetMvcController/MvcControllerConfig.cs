using System;
using System.Collections.Generic;

#nullable enable

namespace Genesis.Output.AspNetMvcController
{
    public class MvcControllerConfig : GeneratorConfiguration
    {
        public string DtoSuffix { get; set; } = "Dto";
        public string Language { get; set; } = "C#";
        public bool Overwrite { get; set; } = true;
        public string Preserve { get; set; } = string.Empty;
        public string DepsServiceNamespace { get; set; } = "Genesis.Services";      //BUG: Find where DepsModelNamespace is being used within the Mvc Controller Generator
        public string DepsModelNamespace { get; set; } = "Genesis.Data.Models";  //hmm, why does a controller need to know about models? - bug somewhere
        public string ServiceSuffix { get; set; } = "Service";
        public string DepsDtoNamespace { get; set; } = "Genesis.Common";
        public bool InitNullable { get; set; } = true;
        [Obsolete("Too common to one-off.")] //TODO: Fix this
        public List<string> Injections { get; set; } = new List<string>(); 
    }
}