namespace Genesis
{
    public enum GraphTypes
    { 
        Object,
        Property,
        Method,
        Event,
        Attribute,
        Parameter,
    }
    public abstract class Graph : IGraph
    {
        protected Graph(GraphTypes eventGraphType) 
            => GraphType = eventGraphType;

        public virtual GraphTypes GraphType { get; private set; } = GraphTypes.Object;
    }
}