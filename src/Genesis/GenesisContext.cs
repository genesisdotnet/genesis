using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Genesis
{
    public class GenesisContext
    {
        public GenesisContext()
        {
            Chain = new ExecutionChain(this);
        }
        /*
         This whole IGenesisExecutionResult thing is worthless and annoying. 
        */
        public static GenesisScope Scope { get; } = new GenesisScope();
        public static GenesisContext Current { get; } = new GenesisContext();

        public int ScanCount { get; set; } = 0;

        public List<ObjectGraph> Objects { get; set; } = new List<ObjectGraph>();


        public ExecutionChain Chain { get; }

        public async Task AddObject(ObjectGraph obj)
        {
            if (Objects.Contains(obj)) //TODO: Way better compare needed
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

        public void WriteContextInfo()
        {
            Text.Line();
            Text.DarkMagentaLine("--------------------------------------------------------------------------------".PadLeft(80));
            Text.Line();

            Text.White("Execution Chain: " + "\n\t");
            Chain.ForEach(e =>
            {
                Text.CliCommand(e.CommandText, false); Text.White(" -> ");
            });
            Text.Line(); Text.Line();
            Text.White($@"Scans since reset: ");
            if (ScanCount > 0)
                Text.GreenLine(ScanCount.ToString());
            else
                Text.YellowLine(ScanCount.ToString());
            Text.Line();
            Text.White($"Execute an "); Text.FriendlyText("input"); Text.White(" or an "); Text.FriendlyText("output"); Text.White(" using the "); Text.CliCommand("exec"); Text.WhiteLine(" command.");
            Text.Line();
        }
    }
}