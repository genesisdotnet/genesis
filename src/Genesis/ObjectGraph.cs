using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

#nullable enable

namespace Genesis
{
    public class ObjectGraph : Graph, IKeyable
    {
        public ObjectGraph() : base(GraphTypes.Object) { }
        public override GraphTypes GraphType => GraphTypes.Object;
        public string Namespace { get; set; } = string.Empty;
        public List<AttributeGraph> Attributes { get; set; } = new List<AttributeGraph>();
        public List<PropertyGraph> Properties { get; set; } = new List<PropertyGraph>();
        public List<MethodGraph> Methods { get; set; } = new List<MethodGraph>();
        public List<EventGraph> Events { get; set; } = new List<EventGraph>();
        public string Name { get; set; } = "OG";
        public bool IsKeyProperty { get; set; }
        public object KeyId { get; set; } = "0";
        public string SourceType { get; set; } = "object";
        public bool IsDefault { get { return Properties.Count == 0 && Methods.Count == 0; } }
        [XmlIgnore]
        public Type BaseType { get; set; } = typeof(object);
        public string BaseTypeFormattedName { get; set; } = string.Empty;
        public bool IsGeneric { get; set; }
        public string[] GenericArgumentTypes { get; set; } = Array.Empty<string>();
        public string KeyDataType { get; set; } = "int";
    }
}
