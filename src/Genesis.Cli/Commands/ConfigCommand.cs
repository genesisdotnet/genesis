using Genesis.Cli.Extensions;
using Genesis.Generation;
using Genesis.Population;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class ConfigCommand : GenesisCommand
    {
        public override string Name { get => "config"; }
        public override string Description => "Edit an Executor's .Config object or list its current contents (soon)";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputTaskResult();

            if (args.Length == 1 || HelpWasRequested(args)) //just 'gen' or 'gen --help,-?'
            {
                Console.WriteLine("Usage:");
                Text.White($"\t{Name} "); Text.Green("CommandText"); Text.WhiteLine($@" PropertyName=<value>");
                Text.WhiteLine($"\t\tSet the Executor.Config.PropertyName to the value provided.");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.ResetColor();
                Console.WriteLine();

                if (OutputManager.Generators.Count == 0 && InputManager.Populators.Count == 0) //NO Generators Found
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("There are no Executors discovered yet. Run a '");
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("scan");
                    Console.ResetColor();
                    Console.WriteLine("'.");
                }
                else //Something was found
                {
                    foreach (var item in InputManager.Populators)
                    {
                        Text.White("Populator: "); Text.Green($@"{item.CommandText}"); Text.White(" From: "); Text.Cyan($"'{item.GetType().Name}'"); Text.WhiteLine($"\t\t{ item.Description} ");
                    }
                    foreach (var item in OutputManager.Generators)
                    {
                        Text.White("Generator: "); Text.Green($@"{item.CommandText}"); Text.White(" From: "); Text.Cyan($"'{item.GetType().Name}'"); Text.WhiteLine($"\t\t{ item.Description} ");
                    }
                }
                result.Success = true;
                result.Message = string.Empty;
            }
            else
            {
                var generator = OutputManager.Generators.Find(g => g.CommandText.Trim().ToLower() == args[1].Trim().ToLower());
                var populator = InputManager.Populators.Find(p => p.CommandText.Trim().ToLower() == args[1].Trim().ToLower());

                var chunks = args[2].Split('=');
                var propertyName = chunks[0];

                if (generator != null )
                {

                    if (!await generator.EditConfig(chunks[0], chunks[1]))
                    {
                        Text.RedLine("Couldn't update value");
                    }
                    else
                    {
                        result.Success = true;
                    }
                } else if (populator != null)
                {
                    if (!await populator.EditConfig(chunks[0], chunks[1]))
                    {
                        Text.RedLine("Couldn't update value");
                    }
                    else
                    {
                        result.Success = true;
                    }
                }
                else
                {
                    Console.Write("'");
                    Text.Green(args[1]);
                    Console.ResetColor();
                    Console.Write("'");
                    Console.WriteLine(" is not a known Executor. (Populator or Generator)");

                    result.Message = "Invalid Executor";
                }
            }
            return await Task.FromResult(result);
        }
    }
}