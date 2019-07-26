namespace Genesis.Input
{
    public class InputGenesisExecutionResult : BlankGenesisExecutionResult, IGenesisExecutionResult
    {
        public override bool Success { get; set; } = false;
        public override string Message { get; set; } = "Nothing of interest really. Got some stuff from a thing.";
    }
}