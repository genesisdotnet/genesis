using Genesis;
using Genesis.Output.Templates;
using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Output
{
    /// <summary>
    /// The primary class responsible for writing out generated code and imported via the core Extensibility Framework (MEF) 
    /// </summary>
    [Export(nameof(IOutputExecutor), typeof(IOutputExecutor))]
    public abstract class OutputExecutor : GenesisExecutor<OutputGenesisExecutionResult>, IOutputExecutor
    {
        private readonly string commandText = "newgenerator";
        public override string CommandText { get => commandText; }

        private readonly string description = "Enter a description for this OutputExecutor";
        public override string Description { get => description; }

        private readonly string friendlyName = "FriendlyName Can Have Spaces";

        public IList<IOutputDependency> _deps = new List<IOutputDependency>();

        public override string FriendlyName { get => friendlyName; }

        public virtual IGeneratorTemplate Template { get; set; } = new StringTemplate("Template contains text loaded from a source"); //for now
        public virtual string OutputPath { get; set; } = Path.Combine(Environment.CurrentDirectory, "Output");

        public IOutputConfiguration Configuration { get; set; } = new GeneratorConfiguration();

        public IList<IOutputDependency> Dependencies { get => _deps; }

        public IDictionary<string, Func<GenesisContext, ObjectGraph, string>> Actions { get; } = new Dictionary<string, Func<GenesisContext, ObjectGraph, string>>();

        public Task<bool> DepositDependencies(string outputRoot = "") 
        {
            try
            {
                foreach (var dependency in _deps)
                {
                    var modifiedDep = OnBeforeWriteDependency(this, new DependencyEventArgs((GenesisDependency)dependency));

                    var loc = ((GeneratorConfiguration)Configuration).DepsPath;

                    var directory = !string.IsNullOrEmpty(loc) 
                                        ? loc : !string.IsNullOrEmpty(outputRoot)
                                            ? outputRoot
                                            : ((GeneratorConfiguration)Configuration).OutputPath;

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    var filename = dependency.PathFragment.TrimStart('.').TrimStart('/').TrimStart('\\');
                    
                    var path = Path.Combine(directory, filename);

                    File.WriteAllText(path, modifiedDep.Contents, Encoding.UTF8);

                    Text.Gray($"Wrote ["); Text.DarkYellow(path); Text.GrayLine("]");
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

        public Task AttachDependencies(IList<IOutputDependency> deps)
        {
            _deps = deps;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Return a modified dependency
        /// </summary>
        protected virtual GenesisDependency OnBeforeWriteDependency(object sender, DependencyEventArgs e) 
            => e.Dependency;
    }
}
