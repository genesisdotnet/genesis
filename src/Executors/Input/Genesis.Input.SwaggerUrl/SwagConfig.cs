using Genesis.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Input.SwaggerUrl
{
    public class SwagConfig : InputConfiguration
    {
        public string Address { get; set; } = string.Empty;
        public string OutputNamespace { get; set; } = "Genesis.Rest.Clients";
        public string LanguageVersion { get; set; } = "CSharp8";
    }
}
