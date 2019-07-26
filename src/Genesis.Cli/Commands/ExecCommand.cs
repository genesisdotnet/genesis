using Genesis.Cli.Extensions;
using Genesis.Output;
using Genesis.Input;
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

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var r = new BlankGenesisExecutionResult();

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

            var exe = GetExecutor(args[1]);

            if(exe != null) //executor name
            {
                await exe.Execute(genesis, args);
                return new BlankGenesisExecutionResult(); //whatever for now
            }
            else if (args[1] == "chain")
            {
                var errorCount = 0;

                foreach (var e in genesis.Chain.Execute(args))
                {
                    if (!e.Success) //Executors have to return a result with true
                        errorCount++;
                }

                if (errorCount == 0)
                {
                    Text.SuccessGraffiti();
                    return new OutputGenesisExecutionResult { Success = true };
                }

                if (errorCount < genesis.Chain.Count)
                {
                    Text.WarningGraffiti();
                    return new OutputGenesisExecutionResult { Success = false };
                }

                if (errorCount == genesis.Chain.Count)
                {
                    Text.ErrorGraffiti();
                    return new OutputGenesisExecutionResult { Success = false };
                }

                return new BlankGenesisExecutionResult(); //whatever for now
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
