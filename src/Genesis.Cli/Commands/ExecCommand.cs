using Genesis.Cli.Extensions;
using Genesis.Generation;
using Genesis.Population;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class ExecCommand : GenesisCommand
    {
        public override string Name { get => "exec"; } //I keep typing 'exec' whatever, and it's annoying. 

        public override string Description => "Execute an IGenesisExecutor by alias (Populator or Generator)";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            //no need to validate the state?
            
            //TODO: Maybe search known executors (pops and gens) and run the one found... weird but scriptable
            //TODO: Make a generic executor for massaging data?
            //TODO: If a Generator or a Populator isn't set, warn etc. It pukes with NullReferenceException otherwise edges * confusing to read
            if (args[1] == "pop") //cheesy
                return (InputTaskResult) await genesis.Populator.Execute(genesis, string.Empty);
            else if (args[1] == "gen")
                return (OutputTaskResult) await genesis.Generator.Execute(genesis, string.Empty);
            else
                throw new InvalidOperationException("Invalid execution: " + args[0] + " " + args[1]);
        }
    }
}
