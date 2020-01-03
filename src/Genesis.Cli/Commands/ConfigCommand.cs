using Genesis.Cli.Extensions;
using Genesis.Output;
using Genesis.Input;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Genesis.Cli.Commands
{
    public class ConfigCommand : GenesisCommand
    {
        public override string Name { get => "config"; }
        public override string Description => "Edit Executor configurations";

        protected override Task OnHelpRequested(string[] args)
        {
            Console.WriteLine("Usage:");
            Text.White($"\t{Name} "); Text.Green("CommandText"); Text.WhiteLine($@" PropertyName=<value>");
            Text.WhiteLine($"\tSet the Executor.Config.PropertyName to the value provided.");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.ResetColor();
            Console.WriteLine();

            if (OutputManager.Outputs.Count == 0 && InputManager.Inputs.Count == 0) //NO Current Found
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
                foreach (var item in InputManager.Inputs)
                {
                    Text.White("Input: "); Text.Green($@"{item.CommandText}"); Text.White(" From: "); Text.Cyan($"'{item.GetType().Name}'"); Text.WhiteLine($"\t\t{ item.Description} ");
                }
                foreach (var item in OutputManager.Outputs)
                {
                    Text.White("OutputExecutor: "); Text.Green($@"{item.CommandText}"); Text.White(" From: "); Text.Cyan($"'{item.GetType().Name}'"); Text.WhiteLine($"\t\t{ item.Description} ");
                }
            }

            return base.OnHelpRequested(args);
        }
        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputGenesisExecutionResult { Success = true };

            if (args.Length == 1) //config
            {
                await OnHelpRequested(args);
                return result;
            }
            else if (args.Length == 2) //config executorname
            {
                WriteExecutorDetails(args[1]);
                return result;
            }
            else
            {
                try
                {
                    var generator = OutputManager.Outputs.Find(g => g.CommandText.Trim().ToLower().Equals(args[1].Trim().ToLower(), StringComparison.Ordinal));
                    var populator = InputManager.Inputs.Find(p => p.CommandText.Trim().ToLower() == args[1].Trim().ToLower());

                    var chunks = args[2].Split('='); //TODO: terse
                    var propertyName = chunks[0];
                    var val = args[2].Substring(propertyName.Length + 1);
                    object? propertyValue = null;

                    if (int.TryParse(val, out var number))
                        propertyValue = number;
                    else
                        propertyValue = val;

                    if (generator != null)
                    {
                        if (!await generator.EditConfig(propertyName, propertyValue))
                        {
                            Text.RedLine("Couldn't update value");
                        }
                        else
                        {
                            result.Success = true;
                        }
                    }
                    else if (populator != null)
                    {
                        if (!await populator.EditConfig(propertyName, propertyValue))
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
                        Text.CliCommand(args[1]);
                        Text.Red(" is not a known Executor. ("); Text.FriendlyText("Input", false); Text.Red(" or "); Text.FriendlyText("Output"); Text.RedLine(").");

                        result.Message = "Invalid Executor";
                    }
                }
                catch (IndexOutOfRangeException ioor)
                {
                    result.Message = ioor.Message;
                    Text.RedLine("Invalid arguments");
                }

                return await Task.FromResult(result);
            }
        }

        private void WriteExecutorDetails(string executorName)
        {
            var exe = GetExecutor(executorName);

            if(exe == null)
            {
                Text.DarkYellowLine("No executor found with that name.");
                return;
            }

            exe.DisplayConfiguration();

            Text.Line();
        }
    }
}