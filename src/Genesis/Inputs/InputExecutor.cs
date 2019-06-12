using Genesis;
using System.Composition;
using System.Threading.Tasks;

namespace Genesis.Input
{
    /// <summary>
    /// The primary class responsible for writing out generated code and imported via the core Extensibility Framework (MEF) 
    /// </summary>
    [Export(nameof(IInputExecutor), typeof(IInputExecutor))]
    public abstract class InputExecutor : GenesisExecutor<InputTaskResult>, IInputExecutor
    {
        private readonly string commandText = "newpopulator";
        public override string CommandText { get => commandText; }

        private readonly string description = "Enter a description for this InputExecutor";
        public override string Description { get => description; }

        private readonly string friendlyName = "FriendlyName Can Have Spaces";
        public override string FriendlyName { get => friendlyName; }

        public IInputConfiguration Configuration { get; set; } = new InputConfiguration();

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            return await Task.FromResult(new InputTaskResult());
        }
    }
}
