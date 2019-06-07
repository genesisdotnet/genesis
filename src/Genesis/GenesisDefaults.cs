using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Genesis
{
    public class GenesisDefaults
    {
        public static readonly string OutputPath = Path.Combine(Environment.CurrentDirectory, "Output");
        public static readonly string Namespace = "Default.Namespace";
        public static readonly string LibraryExtension = ".dll";
        public static readonly string TemplateExtension = ".template";
        public static readonly string UnknownSourceType = "UNKNOWN";
    }
}
