using Genesis.Cli.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class ExitCommand : GenesisCommand
    {
        public override string Name { get => "exit"; }

        public override string Description => "Exit the app";

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            Debug.WriteLine($@"{GetType().Name}.{nameof(Execute)}");
            Console.WriteLine("Exiting");

            Environment.Exit(0); //untz

            return await Task.FromResult(new BlankGenesisExecutionResult());
        }
    }
}
