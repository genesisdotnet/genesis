using System;
using System.Collections.Generic;
using System.Composition;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Executors
{
    /// <summary>
    /// The primary class responsible for general operations that may / may not be dependent on the object graph
    /// </summary>
    [Export(nameof(IGeneralExecutor), typeof(IGeneralExecutor))]
    public abstract class GeneralExecutor : GenesisExecutor<IGenesisExecutionResult>, IGeneralExecutor
    {
        private readonly string commandText = "executor";
        public override string CommandText { get => commandText; }

        private readonly string description = "Enter a description for this Executor";
        public override string Description { get => description; }

        private readonly string friendlyName = "FriendlyName Can Have Spaces";
        public override string FriendlyName { get => friendlyName; }

        public IGeneralConfiguration Configuration { get; set; }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            return await Task.FromResult(new BlankGenesisExecutionResult());
        }
    }
}
