using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    public class ParameterGraph : Graph
    {
        public Type DataType { get; set; }
        public object Value { get; set; }
        public override GraphTypes GraphType => GraphTypes.Parameter;
    }
}
