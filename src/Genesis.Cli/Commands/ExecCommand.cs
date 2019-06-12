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
                Text.CliCommand("command");
                Text.White(" or ");
                Text.Yellow("chain");
                Text.WhiteLine(" to execute the chain");
                Text.Line();

                return r;
            }

            if (args[1] == "pop") //cheesy
                return (InputTaskResult)await genesis.Populator.Execute(genesis, args);
            else if (args[1] == "gen")
                return (OutputTaskResult)await genesis.Generator.Execute(genesis, args);
            else if (args[1] == "chain")
            {
                var singleError = false;
                var allErrors = true;
                foreach (var e in genesis.Chain.Execute(args))
                {
                    if (!e.Success)
                    {
                        singleError = true;
                        allErrors = false;
                    }
                }

                if (!singleError && !allErrors)
                {
                    Text.SuccessGraffiti();
                    return new OutputTaskResult { Success = (!singleError && !allErrors) };
                }

                if (singleError)
                {
                    Text.WarningGraffiti();
                    return new OutputTaskResult { Success = singleError };
                }

                if (allErrors)
                {
                    Text.ErrorGraffiti();
                    return new OutputTaskResult { Success = !allErrors && singleError };
                }

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
