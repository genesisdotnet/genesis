using Genesis.Cli.Extensions;
using Genesis.Output;
using Genesis.Input;
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
        public override string Description => "Scan the filesystem for Producers and Outputs within assemblies";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new BlankTaskResult() { Success = true, Message = "" };

            Text.Line();
            Text.YellowLine("Scanning for Inputs:");
            await InputManager.InitializeInputsAsync(true);

            Text.Line();

            Text.YellowLine("Scanning for Outputs:");
            await OutputManager.InitializeGeneratorsAsync(true);

            Text.Line();

            Console.ForegroundColor = (InputManager.Inputs.Count > 0) ? ConsoleColor.Green : ConsoleColor.Yellow;
            Text.White($@"{InputManager.Inputs.Count}");
            Text.WhiteLine($" Potential Inputs(s)");

            Console.ForegroundColor = (OutputManager.Outputs.Count > 0) ? ConsoleColor.Green : ConsoleColor.Yellow;
            Text.White($"{OutputManager.Outputs.Count}");
            Text.WhiteLine(" Possible Outputs(s)");

            Text.Line();
            Text.White("The ");Text.Green("green"); Text.WhiteLine(" text is how you reference an Executor. ");
            Text.Line();

            genesis.ScanCount++;

            return await Task.FromResult(result);
        }
    }
}
