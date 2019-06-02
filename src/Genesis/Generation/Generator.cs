using Genesis.Cli;
using Genesis.Generation.Templates;
using System;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Generation
{
    /// <summary>
    /// The primary class responsible for writing out generated code and imported via the core Extensibility Framework (MEF) 
    /// </summary>
    [Export(nameof(IGenerator), typeof(IGenerator))]
    public abstract class Generator : GenesisExecutor<OutputTaskResult>, IGenerator
    {
        private readonly string commandText = "newgenerator";
        public override string CommandText { get => commandText; }

        private readonly string description = "Enter a description for this Generator";
        public override string Description { get => description; }

        private readonly string friendlyName = "FriendlyName Can Have Spaces";
        public override string FriendlyName { get => friendlyName; }

        public virtual IGeneratorTemplate Template { get; set; } = new StringTemplate("Template contains text loaded from a source"); //for now
        public virtual string OutputPath { get; set; } = Path.Combine(Environment.CurrentDirectory, "Output");

        public IGeneratorConfiguration Configuration { get; set; } = new GeneratorConfiguration();
    }
}
