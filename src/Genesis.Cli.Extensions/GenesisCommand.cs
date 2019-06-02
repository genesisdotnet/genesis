using Genesis.Generation;
using Genesis.Population;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Genesis.Cli.Extensions
{
    public abstract class GenesisCommand : IGenesisCommand
    {
        public GenesisCommand() {}

        public abstract string Name { get; }
        public virtual string HelpTemplate { get; } = "-?|-h|--help";
        public abstract Task<ITaskResult> Execute(GenesisContext genesis, string[] args);
        public virtual string Usage { get; } = "command parameter [[0] [1] [2]]"; //or something
        public abstract string Description { get; }

        public virtual async Task InitializeAsync()
        {
            Debug.WriteLine($@"{GetType().Name}.{nameof(InitializeAsync)}");
            await Task.CompletedTask;
        }

        public IPopulator GetPopulator(string populatorName = "nope")
        {
            foreach(var populator in InputManager.Populators)
            {
                if (populatorName.ToLower().Trim() == populator.CommandText.ToLower())
                    return populator;
            }
            throw new Exception($"Invalid Populator name '{populatorName}'");
        }

        public IGenerator GetGenerator(string generatorName = "nope")
        {
            foreach (var generator in OutputManager.Generators)
            {
                if (generatorName.ToLower().Trim() == generator.CommandText.ToLower())
                    return generator;
            }
            throw new Exception($"Invalid Generator name '{generatorName}'");
        }

        #region Helper Methods

        private const string helpArguments = "-h,-?,--help";
        private string[] HelpArguments { get => helpArguments.Split(','); }

        /// <summary>
        /// Checks the arguments for a '-h', --help, or -?
        /// </summary>
        /// <param name="args">string array of arguments from the REPL or script</param>
        /// <returns>true if a help option/argument was matched, otherwise false</returns>
        protected bool HelpWasRequested(string[] args)
            => (args.Length == 2 && HelpArguments.Contains(args[1].ToLower()));

        #endregion
    }
}