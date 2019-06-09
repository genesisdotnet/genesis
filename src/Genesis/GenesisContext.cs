using Genesis.Generation;
using Genesis.Population;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Genesis
{
    public class GenesisScope //INotifyPropertyChanged?
    {
        public string PromptString { get; private set; }
        public List<IGenesisExecutor<ITaskResult>> Executors { get; } = new List<IGenesisExecutor<ITaskResult>>();

        private IGenesisExecutor<ITaskResult> _currentExecutor;
        public IGenesisExecutor<ITaskResult> CurrentTask
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

    public class GenesisContext
    {
        public GenesisContext()
        {
            Chain = new ExecutionChain(this);
        }
        /*
         This whole ITaskResult thing is worthless and annoying. 
        */
        public static GenesisScope Scope { get; } = null;
        public static GenesisContext Current { get; } = new GenesisContext();

        private bool hasAllComponentsConfigured = false;

        private IPopulator currentPopulator = null; //should these be ICollections?
        private IGenerator currentGenerator = null;

        public int ScanCount { get; set; } = 0;

        public IPopulator Populator { get { return currentPopulator; } set { currentPopulator = value; } }
        public IGenerator Generator { get { return currentGenerator; } set { currentGenerator = value; } }
        public List<ObjectGraph> Objects { get; set; } = new List<ObjectGraph>();
        public bool HasAllComponentsConfigured { get => hasAllComponentsConfigured; }
        public ExecutionChain Chain { get; private set; } 

        public async Task AddObject(ObjectGraph obj)
        {
            if (Objects.Contains(obj))
            {
                Text.DarkYellowLine($"{obj.Name} exists, not adding.");
                return;
            }

            Objects.Add(obj);

            Text.White($"Adding Object '"); Text.DarkMagenta(obj.Name); Text.WhiteLine("'");

            await Task.CompletedTask;
        }

        public void ClearObjects()
            => Objects.Clear();

        public async Task<ITaskResult> ExecuteConfiguration(string[] args = null)
        {
            ITaskResult result = new BlankTaskResult();
            
            //TODO: Use an Aggregate Exception here

            if (Populator == null)
            {
                Text.RedLine("No Populator configured."); Text.White("Use '"); Text.Green("gen"); Text.WhiteLine("' to set the current Populator");
            }
            else
            {
                Console.Write($"Executing... '");
                Text.Cyan(Populator.FriendlyName);
                Text.WhiteLine("' with its configuration... ");

                try
                {
                    result = await Populator.Execute(this, args);
                    Text.GreenLine("OK");
                }
                catch (Exception popEx)
                {
                    Text.RedLine("ERROR");
                    Text.RedLine($"{popEx.Message}");

                    result = new ErrorTaskResult { Success = false };
                    return result;
                }
            }

            if (Objects.Count == 0 || Objects[0].IsDefault) //no pop and/or no gen
                Text.RedLine("No objects exist yet");
            else
                Text.GreenLine($"There are {Objects.Count} objects populated.");

            if (Generator == null)
            {
                Text.RedLine("No Generator configured.");
                Text.White("Use '"); Text.Green("gen"); Text.WhiteLine("' to set the current Generator");
            }
            else
            {
                Text.White($"Executing '");
                Text.Green(Generator.CommandText);
                Text.WhiteLine($"'. With output to {Generator.Configuration.OutputPath}");
                
                try
                {
                    result = await Generator.Execute(this, args); //TODO: Pass args
                    Text.GreenLine("OK");
                    return result;
                }
                catch (Exception popEx)
                {
                    Text.RedLine("ERROR");
                    Text.RedLine($"{popEx.Message}");

                    return new ErrorTaskResult { Success = false, Message = popEx.Message };
                }
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Validates the configuration and sets the HasAllComponentsConfigured property
        /// </summary>
        public void ValidateGenesisContext()
        {
            hasAllComponentsConfigured = (
                Populator != null
                && Generator != null
            //for now
            );
        }

        public async Task ConfigurePopulator(IPopulator newPopulator)
        {
            currentPopulator = newPopulator;

            await Task.CompletedTask;
        }

        public async Task ConfigureGenerator(IGenerator newGenerator)
        {
            currentGenerator = newGenerator;

            await Task.CompletedTask;
        }

        public void WriteContextInfo()
        {
            Console.WriteLine();
            Console.Write($@"Current Input is:");

            if (Populator != null)
            {
                Text.Cyan($" '{Populator.FriendlyName}'"); Text.White(" ("); Text.Green(Populator.CommandText); Text.WhiteLine(") ");
            }
            else
                Text.YellowLine($" not configured");

            Console.Write($@"Current Output is:");

            if (Generator != null)
            {
                Text.Cyan($" '{Generator.FriendlyName}'"); Text.White(" ("); Text.Green(Generator.CommandText); Text.WhiteLine(") ");
            }
            else
                Text.YellowLine($" not configured");

            Console.WriteLine();
            Console.Write($@"Scans since reset: ");
            if (ScanCount > 0)
                Text.GreenLine(ScanCount.ToString());
            else
                Text.RedLine(ScanCount.ToString());

            Text.Line();

            Text.White($"Execute an ");
            Text.FriendlyText("input");
            Text.White(" or an ");
            Text.FriendlyText("output");
            Text.White(" using the ");
            Text.Command("exec");
            Text.WhiteLine(" command.");

            Text.Line();
        }
    }
}
