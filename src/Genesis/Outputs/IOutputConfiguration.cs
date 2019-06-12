namespace Genesis.Output
{
    public interface IOutputConfiguration
    {
        string Namespace { get; set; }
        string OutputPath { get; set; }
    }
}
