namespace Genesis.Input
{
    public class InputTaskResult : BlankTaskResult, ITaskResult
    {
        public override bool Success { get; set; } = false;
        public override string Message { get; set; } = "Nothing of interest really. Got some stuff from a thing.";
    }
}