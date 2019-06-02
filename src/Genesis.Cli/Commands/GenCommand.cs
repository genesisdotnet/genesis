using Genesis.Cli.Extensions;
using Genesis.Generation;
using System;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class GenCommand : GenesisCommand
    {
        public override string Name { get => "gen"; }
        public override string Description => "Configure a Generator or list current Known Generators";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputTaskResult();

            if (args.Length == 1 || HelpWasRequested(args)) //just 'gen' or 'gen --help,-?'
            {
                Console.WriteLine("Usage:");
                Console.WriteLine($"\t{Name} <KnownGeneratorName>\t\t\tSet the current generator to the type provided.");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\t'{Name} GeneratorName'");
                Console.ResetColor();
                Console.WriteLine();

                if (OutputManager.Generators.Count == 0) //NO Generators Found
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("There are no generators discovered yet. Run a '");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("scan");
                    Console.ResetColor();
                    Console.WriteLine("'.");
                }
                else //Generators were found
                {
                    Console.WriteLine("Known Generators:");
                    foreach (var item in OutputManager.Generators)
                    {
                        Text.White("Command: "); Text.Green($@"{item.CommandText}"); Text.White(" From: "); Text.Cyan($"'{item.GetType().Name}'"); Text.WhiteLine($"\t\t{ item.Description} ");
                    }
                }
                result.Success = true;
                result.Message = string.Empty;
            }
            else
            { 
                var generator = OutputManager.Generators.Find(g => g.CommandText.Trim().ToLower() == args[1].Trim().ToLower());
                if(generator != null)
                {
                    await genesis.ConfigureGenerator(generator);
                    Text.White($@"The current Generator is now: "); Text.CyanLine($"'{generator.GetType().Name}'");
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
                    Console.WriteLine(" is not a known Generator name.");

                    result.Message = "Invalid Generator.Name";
                }
            }

            return await Task.FromResult(result);
        }
    }
}
