namespace Genesis
{
    public interface ITaskResult 
    {
        bool Success { get; set; }
        string Message { get; set; }
    }
}