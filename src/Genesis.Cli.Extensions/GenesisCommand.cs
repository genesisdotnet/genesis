using Genesis.Output;
using Genesis.Input;
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

        public Task InitializeAsync(string[] args)
        {
            Debug.WriteLine($@"{GetType().Name}.{nameof(InitializeAsync)}");

            return OnInitializing(args);
        }

        /// <summary>
        /// Tells the command to check and see if they should write a help command to the screen
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task ProcessHelpCommand(string[] args)
        {
            if (HelpWasRequested(args))
            {
                Debug.WriteLine($@"{GetType().Name}.{nameof(ProcessHelpCommand)}");
                OnHelpRequested(args);
            }

            return Task.CompletedTask;
        }

        protected virtual Task OnInitializing(string[] args)
        {
            //dump dependencies from a convention?
            return Task.CompletedTask;
        }

        /// <summary>
        /// Occurs when - command occurs with no args in [1] or later, or has -h, -help, --help
        /// Override this to write out a help page
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual Task OnHelpRequested(string[] args)
        {
            return Task.CompletedTask;
        }

        public IGenesisExecutor<ITaskResult> GetExecutor(string executorName)
        {
            var exe = (IGenesisExecutor<ITaskResult>)InputManager.Inputs.Where(w => w.CommandText.Equals(executorName, StringComparison.Ordinal)).SingleOrDefault();

            if (exe == null)
                exe = OutputManager.Outputs.Where(w => w.CommandText.Equals(executorName, StringComparison.Ordinal)).SingleOrDefault();

            return exe;
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