using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    public class EventGraph : Graph
    {
        public string Name { get; set; } = "SomethingHappened";
        public string DelegateFormattedName { get; set; } = "EventHandler";
        public EventGraph() : base(GraphTypes.Event)
        {

        }
    }
}
