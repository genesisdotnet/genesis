using Genesis.Cli.Extensions;
using Genesis.Generation;
using Genesis.Population;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class ScanCommand : GenesisCommand
    {
        public override string Name { get => "scan"; }
        public override string Description => "Scan the filesystem for Producers and Generators within assemblies";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new BlankTaskResult() { Success = true, Message = "" };

            Console.WriteLine();
            Console.WriteLine("Scanning Populators:");
            await InputManager.InitializePopulatorsAsync(true);

            Console.WriteLine();

            Console.WriteLine("Scanning Generators:");
            await OutputManager.InitializeGeneratorsAsync(true);

            Console.WriteLine();

            Console.ForegroundColor = (InputManager.Populators.Count > 0) ? ConsoleColor.Green : ConsoleColor.Yellow;
            Console.Write($@"{InputManager.Populators.Count}");
            Console.ResetColor();
            Console.WriteLine($" Populator(s)");

            Console.ForegroundColor = (OutputManager.Generators.Count > 0) ? ConsoleColor.Green : ConsoleColor.Yellow;
            Console.Write($"{OutputManager.Generators.Count}");
            Console.ResetColor();
            Console.WriteLine(" Generator(s)");

            genesis.ScanCount++;

            return await Task.FromResult(result);
        }
    }
}
