using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Genesis
{
    public class GenesisScope
    {
        public string PromptString { get; private set; }
        public List<IGenesisExecutor<IGenesisExecutionResult>> Executors { get; } = new List<IGenesisExecutor<IGenesisExecutionResult>>();

        private IGenesisExecutor<IGenesisExecutionResult> _currentExecutor;
        public IGenesisExecutor<IGenesisExecutionResult> CurrentTask
        {
            get => _currentExecutor;
            private set
            {
                _currentExecutor = value;
                PromptString = (_currentExecutor != null)
                                ? "/" + _currentExecutor.CommandText
                                : string.Empty;
            }
        }
        public async Task Configure(string propertyName, object value)
        {
            try
            {
                if (_currentExecutor == null)
                    throw new Exception("You're not within a scope. "); //TODO: Elaborate how to set a scope

                _ = await CurrentTask.EditConfig(propertyName, value?.ToString() ?? string.Empty); //for now

            }
            catch (Exception e)
            {
                Text.RedLine($@"{e.Message}");
            }
        }
        public async Task ExitScope()
        {
            CurrentTask = null;

            await Task.CompletedTask;
        }
    }
}
