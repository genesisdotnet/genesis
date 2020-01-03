namespace Genesis.Output
{
    public interface IOutputDependency
    {
        string PathFragment { get; }
        string Contents { get; }
        string ObjectName { get; }
    }
}
