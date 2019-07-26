namespace Genesis
{
    public interface IGenesisExecutionResult 
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}