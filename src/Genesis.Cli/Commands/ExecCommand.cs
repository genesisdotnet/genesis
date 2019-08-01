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
            var tmp = (args.Length==1) ? args[0] : args[1];

            var exe = GetExecutor(tmp);

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
                var temp = GetExecutor(args[1]);

                if (temp == null)
                    throw new InvalidOperationException("Invalid execution: " + args[0] + " " + args[1]);

                return await temp.Execute(genesis, args);
            }
        }
    }
}
