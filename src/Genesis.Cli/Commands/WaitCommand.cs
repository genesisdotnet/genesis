using Genesis.Cli.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    class WaitCommand : GenesisCommand
    {
        public override string Name { get => "wait"; }

        public override string Description => "Pause (a script) until user input";

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            Text.DarkYellowLine("Paused... press the any key");
            Console.ReadKey();

            return await Task.FromResult(new BlankGenesisExecutionResult());
        }
    }
}
