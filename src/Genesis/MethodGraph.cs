using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Genesis
{
    public enum MethodVisibilities
    {
        Public,
        Private,
        Internal,
        Protected
    }

    public class MethodGraph : Graph
    {
        public MethodGraph() : base(GraphTypes.Method) { }
        [XmlIgnore]
        public Type ReturnDataType { get; set; }
        public MethodVisibilities MethodVisibility { get; set; } = MethodVisibilities.Public;
        public List<ParameterGraph> Parameters { get; } = new List<ParameterGraph>();
        public string Name { get; set; }
        public bool HasGenericParams { get; set; }
        public bool IsGeneric { get; set; }
        public string ReturnTypeFormattedName { get; set; }
#pragma warning disable CA1819 // Properties should not return arrays
        public string[] FormattedGenericArguments { get; set; } = Array.Empty<string>();
#pragma warning restore CA1819 // Properties should not return arrays
    }
}
