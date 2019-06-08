using Genesis.Generation;
using Genesis.Population;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Genesis.Cli
{
    public class GenesisScope //INotifyPropertyChanged?
    {
        public string PromptString { get; private set; }
        public List<IGenesisExecutor<ITaskResult>> Executors { get; } = new List<IGenesisExecutor<ITaskResult>>();

        private IGenesisExecutor<ITaskResult> _currentExecutor;
        public IGenesisExecutor<ITaskResult> CurrentTask {
            get =>_currentExecutor;
            private set {
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
            catch(Exception e)
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

        public async Task AddObject(ObjectGraph obj)
        {
            if (Objects.Contains(obj))
            {
                Text.DarkYellowLine($"{obj.Name} exists, not adding (use --force) to overwrite.");
                await Task.CompletedTask;
            }

            Objects.Add(obj);

            Text.GreenLine($"Adding ObjectGraph '{obj.Name}'");

            await Task.CompletedTask;
        }

        public void ClearObjects()
            => Objects.Clear();
        
        public async Task<BlankTaskResult> ExecuteConfiguration(string[] args = null)
        {
            var result = new BlankTaskResult();
            //var x = new ExecutionAggregateException();

            //TODO: Use an Aggregate Exception here

            //if (!HasAllComponentsConfigured)
            //{
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
                    await Populator.Execute(this, string.Empty); //TODO: Pass args
                    Text.GreenLine("OK");
                    result.Success = true;
                }
                catch (Exception popEx)
                {
                    Text.RedLine("ERROR");
                    Text.RedLine($"{popEx.Message}");

                    result.Success = false;
                    //result.Message = popEx.Message;
                    return result;
                }
            }
            //TODO: lame check that will blow up
            if (Objects.Count == 0 || Objects[0].IsDefault) //no pop and/or no gen
            {
                Text.RedLine("Objects is empty");
                result.Message = "No data in ObjectGraph";
                result.Success = false;
            }
            else
            {
                Text.GreenLine("GenesisContext data is present");
                result.Success = true;
            }

            if (Generator == null)
            {
                Text.RedLine("No Generator configured."); Text.White("Use '"); Text.Green("gen"); Text.WhiteLine("' to set the current Generator");
            }
            else
            {
                Text.White($"Executing '");
                Text.Cyan(Generator.FriendlyName);
                Text.White("' with its configuration... ");

                try
                {
                    //NOTE: presumes whatever implementation isn't going to screw with the terminal 
                    await Generator.Execute(this, string.Empty); //TODO: Pass args
                    Text.GreenLine("OK");
                    result.Success = true;
                }
                catch (Exception popEx)
                {
                    Text.RedLine("ERROR");
                    Text.RedLine($"{popEx.Message}");

                    result.Success = false;
                    result.Message = popEx.Message;
                    return result;
                }
            }
            result.Message = $"Execution performed {(result.Success ? "successfully" : "with errors")}";
            //}
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
            Console.Write($@"Current Populator is:");

            if (Populator != null)
            {
                Text.Cyan($" '{Populator.FriendlyName}'"); Text.White(" ("); Text.Green(Populator.CommandText); Text.WhiteLine(") ");
            }
            else
                Text.RedLine($" not configured");

            Console.Write($@"Current Generator is:");

            if (Generator != null)
            {
                Text.Cyan($" '{Generator.FriendlyName}'"); Text.White(" ("); Text.Green(Generator.CommandText); Text.WhiteLine(") ");
            }
            else
                Text.RedLine($" not configured");

            Console.WriteLine();
            Console.Write($@"Scans since reset: ");
            if (ScanCount > 0)
                Text.GreenLine(ScanCount.ToString());
            else
                Text.RedLine(ScanCount.ToString());

            Console.WriteLine();
            Console.Write("Use the '");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("pop");
            Console.ResetColor();
            Console.Write("' command to configure a Populator and '");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("gen");
            Console.ResetColor();
            Console.WriteLine("' for a Generator like:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\tpop SomeCoolPopulator");
            Console.ResetColor();
            Console.WriteLine("\t\tor");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\tgen SomeCoolGenerator");
            Console.ResetColor();
        }
    }
}
