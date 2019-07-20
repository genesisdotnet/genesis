using Genesis.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Input.YamlAddress
{
    public class YamlConfig : InputConfiguration
    {
        public string Address { get; set; } = string.Empty;
        public string OutputNamespace { get; set; } = "Genesis.Rest.Clients";
    }
}
