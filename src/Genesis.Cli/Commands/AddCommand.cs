using Genesis.Cli.Extensions;
using Genesis.Output;
using Genesis.Input;
using Genesis.Cli;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class AddCommand : GenesisCommand
    {
        public override string Name { get => "add"; }
        public override string Description => "Add an executor to the chain";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new BlankTaskResult();

            //TODO: Move help text to override in AddCommand
            if (args.Length == 1 || HelpWasRequested(args)) //just 'add' or 'add --help,-?'
            {
                Console.WriteLine("Usage:");
                Console.WriteLine($"\t{Name} <command>\t\tAdds an executor to the end of the Chain.");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\t'{Name} ExecutorName'");
                Console.ResetColor();
                Console.WriteLine();

                if (OutputManager.Outputs.Count == 0 || InputManager.Inputs.Count == 0) //NO Executors found, this still needs combined
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("There are no inputs discovered yet. Run a ");
                    Text.CliCommand("scan");
                    Console.WriteLine(".");
                }
                else 
                {
                    Console.WriteLine("Known Inputs:"); //TODO: Get rid of OutputExecutor / InputExecutor concept
                    foreach (var item in InputManager.Inputs)
                    {
                        Text.White("Input: "); Text.Green($@"{item.CommandText}"); Text.White(" From: "); Text.DarkCyanLine($"{item.GetType().Assembly.GetName().Name}"); 
                    }
                    Console.WriteLine();
                    Console.WriteLine("Known Outputs:"); //TODO: Get rid of OutputExecutor / InputExecutor concept
                    foreach (var item in OutputManager.Outputs)
                    {
                        Text.White("Output: "); Text.Green($@"{item.CommandText}"); Text.White(" From: "); Text.DarkCyanLine($"{item.GetType().Assembly.GetName().Name}");
                    }
                }
                result.Success = true;
            }
            else
            {
                Text.Line();

                var generator = OutputManager.Outputs.Find(g => g.CommandText.Trim().ToLower() == args[1].Trim().ToLower());
                if (generator != null)
                {
                    await genesis.Chain.Append(generator);

                    Text.CliCommand(generator.CommandText); Text.WhiteLine($@" was added to the Chain. There are {genesis.Chain.Count} now.");
                    Text.Line();
                    result.Success = true;
                    return result;
                }

                var populator = InputManager.Inputs.Find(p => p.CommandText.Trim().ToLower() == args[1].Trim().ToLower()); //this is silly
                if (populator != null)
                {
                    await genesis.Chain.Append(populator);

                    Text.CliCommand(populator.CommandText); Text.WhiteLine($@" was added to the Chain. There are {genesis.Chain.Count} now.");
                    Text.Line();
                    result.Success = true;
                    return result;
                }

                Text.White("'");
                Text.Red(args[1]);
                Text.WhiteLine("' is not a known input or output.");
                Text.Line();
            }

            return await Task.FromResult(result);
        }
    }
}
