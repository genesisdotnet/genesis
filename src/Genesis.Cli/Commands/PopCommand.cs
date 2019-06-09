using Genesis.Cli.Extensions;
using Genesis.Population;
using System;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class PopCommand : GenesisCommand
    {
        public override string Name { get => "pop"; }
        public override string Description => "Set the current Populator";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new InputTaskResult();

            if (args.Length == 1 || HelpWasRequested(args)) //just 'gen' or 'gen --help,-?'
            {
                Console.WriteLine("Usage:");
                Console.WriteLine($"\t{Name} <Populator>\t\t\tSet the current populator to the type provided.");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\t'{Name} PopulatorName'");
                Console.ResetColor();
                Console.WriteLine();

                if (InputManager.Inputs.Count == 0) //NO Inputs Found
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("There are no populators discovered yet. Run a '");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("scan");
                    Console.ResetColor();
                    Console.WriteLine("'.");
                }
                else //Inputs were found
                {
                    Console.WriteLine("Known Inputs:");
                    foreach (var item in InputManager.Inputs)
                    {
                        Text.White("Command: "); Text.Green($@"{item.CommandText}"); Text.White(" From: "); Text.Cyan($"'{item.GetType().Name}'"); Text.WhiteLine($"\t\t{ item.Description} ");
                    }
                }
                result.Success = true;
                result.Message = string.Empty;
            }
            else
            {
                var populator = InputManager.Inputs.Find(g => g.CommandText.Trim().ToLower() == args[1].Trim().ToLower());
                if (populator != null)
                {
                    await genesis.ConfigurePopulator(populator);
                    Text.White($@"The current Populator is now: "); Text.CyanLine($"'{populator.GetType().Name}'");
                    result.Success = true;
                    result.Message = string.Empty;
                }
                else
                {
                    Console.Write("'");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(args[1]);
                    Console.ResetColor();
                    Console.Write("'");
                    Console.WriteLine(" is not a known Populator name.");

                    result.Message = "Invalid Populator.Name";
                }
            }

            return await Task.FromResult(result);
        }
    }
}
