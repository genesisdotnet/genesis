namespace Genesis
{
    public enum GraphTypes
    { //maybe? 
        Object,
        Property,
        Method,
        Event,
        Attribute,
        Parameter,
    }
    public abstract class Graph : IGraph
    {
        public virtual GraphTypes GraphType { get; } = GraphTypes.Object;
    }
}