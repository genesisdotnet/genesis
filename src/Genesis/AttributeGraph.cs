using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    public class AttributeGraph : Graph
    {
        public AttributeGraph() : base(GraphTypes.Attribute) { }
        public override GraphTypes GraphType => GraphTypes.Attribute;

    }
}
