using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    public class ObjectGraph : Graph, IKeyable
    {
        public override GraphTypes GraphType => GraphTypes.Object;
        public string Namespace { get; set; }
        public List<AttributeGraph> Attributes { get; set; } = new List<AttributeGraph>();
        public List<PropertyGraph> Properties { get; set; } = new List<PropertyGraph>();
        public List<MethodGraph> Methods { get; set; } = new List<MethodGraph>();
        public string Name { get; set; } = "OG";
        public bool IsKeyProperty { get; set; }
        public object KeyId { get; set; } = "0";
        public string SourceType { get; set; } = "object";
        public bool IsDefault { get { return Properties.Count == 0 && Methods.Count == 0; } }
    }
}
