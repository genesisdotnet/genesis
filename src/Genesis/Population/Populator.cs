using Genesis;
using System.Composition;
using System.Threading.Tasks;

namespace Genesis.Population
{
    /// <summary>
    /// The primary class responsible for writing out generated code and imported via the core Extensibility Framework (MEF) 
    /// </summary>
    [Export(nameof(IPopulator), typeof(IPopulator))]
    public abstract class Populator : GenesisExecutor<InputTaskResult>, IPopulator
    {
        private readonly string commandText = "newpopulator";
        public override string CommandText { get => commandText; }

        private readonly string description = "Enter a description for this Populater";
        public override string Description { get => description; }

        private readonly string friendlyName = "FriendlyName Can Have Spaces";
        public override string FriendlyName { get => friendlyName; }

        public IPopulatorConfiguration Configuration { get; set; } = new PopulatorConfiguration();

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            return await Task.FromResult(new InputTaskResult());
        }
    }
}
