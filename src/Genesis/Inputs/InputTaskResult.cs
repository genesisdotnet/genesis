namespace Genesis.Input
{
    public class InputGenesisExecutionResult : BlankGenesisExecutionResult, IGenesisExecutionResult
    {
        public override bool Success { get; set; } = true;
        public override string Message { get; set; } = "Success";
    }
}