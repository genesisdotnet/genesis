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

        public override string Description => "Execute!";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var r = new BlankTaskResult();

            if(args.Length == 1)
            {
                Text.White("Specify something to execute. A ");
                Text.Command("command");
                Text.White(" or ");
                Text.Yellow("chain");
                Text.WhiteLine(" to execute the chain");

                return r;
            }

            if (args[1] == "pop") //cheesy
                return (InputTaskResult)await genesis.Populator.Execute(genesis, args);
            else if (args[1] == "gen")
                return (OutputTaskResult)await genesis.Generator.Execute(genesis, args);
            else if (args[1] == "chain")
            {
                _ = genesis.Chain.Execute(args);
                return new BlankTaskResult(); //whatever for now
            }
            else
            {
                var tmp = GetExecutor(args[1]);

                if (tmp == null)
                    throw new InvalidOperationException("Invalid execution: " + args[0] + " " + args[1]);

                return await tmp.Execute(genesis, args);
            }
        }
    }
}
