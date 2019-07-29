using System;

namespace Genesis
{
    public class PropertyGraph : Graph, IKeyable //TODO: make observable?
    {
        public override GraphTypes GraphType => GraphTypes.Property;

        public string Name { get; set; }
        public string SourceType { get; set; } = "object";
        public string TypeGuess { get; set; } = "object";  //what is presumed to be needed in a generator
        public bool IsNullable { get; set; } = true;
        public bool IsKeyProperty { get; set; } = false;
        public MethodVisibilities GetterVisibility { get; set; } = MethodVisibilities.Public;
        public MethodVisibilities SetterVisibility { get; set; } = MethodVisibilities.Public;
    }
}