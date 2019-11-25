namespace Genesis.Input.DotNetAssembly
{
    public class AssemblyInputConfig : InputConfiguration
    {
        public string AssemblyPath { get; set; } = "/Temp/TestAssembly.dll";
        public bool OnlyGenesisDecorations { get; set; } = true;
    }
}