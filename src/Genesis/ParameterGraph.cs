using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Genesis
{
    public class ParameterGraph : Graph
    {
        [XmlIgnore]
        public Type DataType { get; set; }
        [XmlIgnore]
        public object Value { get; set; }
        public override GraphTypes GraphType => GraphTypes.Parameter;

        public string Name { get; set; }
        public bool IsOut { get; set; }
        public bool IsOptional { get; set; }
        public int Position { get; set; }
        public string DataTypeFormattedName { get; set; }
        public bool IsIn { get; set; }
        public bool IsGeneric { get; set; }
        public bool IsGenericMethodParameter { get; set; }
        public string DisplayName { get; set; }
        public string[] GenericArgumentFormattedTypeNames { get; set; }
    }
}
