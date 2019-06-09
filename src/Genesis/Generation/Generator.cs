using Genesis;
using Genesis.Generation.Templates;
using System;
using System.Collections.Generic;
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

        public IList<IGenesisDependency> _deps = new List<IGenesisDependency>();

        public override string FriendlyName { get => friendlyName; }

        public virtual IGeneratorTemplate Template { get; set; } = new StringTemplate("Template contains text loaded from a source"); //for now
        public virtual string OutputPath { get; set; } = Path.Combine(Environment.CurrentDirectory, "Output");

        public IGeneratorConfiguration Configuration { get; set; } = new GeneratorConfiguration();

        public IList<IGenesisDependency> Dependencies { get => _deps; }
        public Task<bool> DepositDependencies(string outputRoot = @"Output\") 
        {
            try
            {
                foreach (var dep in _deps)
                {
                    var path = (string.IsNullOrEmpty(outputRoot)) 
                        ? OutputPath + dep.PathFragment 
                        : outputRoot + dep.PathFragment;

                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                    File.WriteAllText(path, dep.Contents, Encoding.UTF8);

                    Text.Gray($"Wrote ["); Text.Yellow(dep.PathFragment); Text.GrayLine("]");
                }
                Text.GreenLine($"Wrote {_deps.Count} dependencies to their respective paths.");
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                Text.RedLine(e.Message);
                return Task.FromResult(false);
            }
        }

        public Task AttachDependencies(IList<IGenesisDependency> deps)
        {
            _deps = deps;

            return Task.CompletedTask;
        }
    }
}
