using Genesis.Cli.Extensions;
using Genesis.Output;
using Genesis.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genesis.Executors;

namespace Genesis.Cli.Commands
{
    public class StatusCommand : GenesisCommand
    {
        public override string Name { get => "status"; }
        public override string Description => "Display information about the configuration";

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            Text.Line();
            Text.WhiteLine("Known Inputs:");

            if (InputManager.Inputs.Count == 0)
            {
                Text.RedLine("\t no inputs found");
            }
            else
            {
                var tmp = new List<string>(args);

                if (tmp.Contains("detailed") || tmp.Contains("--detailed") || tmp.Contains("-d"))
                {
                    foreach (var exe in InputManager.Inputs)
                        DisplayDetail(exe);
                }
                else
                {
                    foreach (var exe in InputManager.Inputs)
                        DisplayQuick(exe);
                }
            }

            Text.Line();
            Text.WhiteLine("Known Outputs:"); 

            if (OutputManager.Outputs.Count == 0)
            {
                Text.RedLine("\t no outputs found");
            }
            else
            {
                var tmp = new List<string>(args);

                if (tmp.Contains("detailed") || tmp.Contains("--detailed") || tmp.Contains("-d"))
                {
                    foreach (var exe in OutputManager.Outputs)
                        DisplayDetail(exe);
                }
                else
                {
                    foreach (var exe in OutputManager.Outputs)
                        DisplayQuick(exe);
                }
            }

            Text.Line();
            Text.WhiteLine("Known Generals:");

            if (GeneralManager.Current.Count == 0)
            {
                Text.RedLine("\t no general executors found");
            }
            else
            {
                var tmp = new List<string>(args);

                if (tmp.Contains("detailed") || tmp.Contains("--detailed") || tmp.Contains("-d"))
                {
                    foreach (var exe in GeneralManager.Current)
                        DisplayDetail(exe);
                }
                else
                {
                    foreach (var exe in GeneralManager.Current)
                        DisplayQuick(exe);
                }
            }

            Console.ResetColor();

            genesis.WriteContextInfo();

            return await Task.FromResult(new BlankGenesisExecutionResult() { Success = true, Message = "" });
        }

        private static void DisplayDetail(IGenesisExecutor<IGenesisExecutionResult> exe)
        {
            Text.White("\tCommand: "); Text.CliCommand(exe.CommandText, false); Text.Line();
            Text.White("\tSource: "); Text.Yellow(exe.GetType().Assembly.GetName().Name); Text.White("."); Text.Blue(exe.GetType().Name); Text.Line();
            Text.White("\tDescription: "); Text.FriendlyText(exe.FriendlyName, false); Text.Line();
            Text.White("\tFile Path: "); Text.GrayLine(exe.GetType().Assembly.Location);
            Text.Line();
        }

        private static void DisplayQuick(IGenesisExecutor<IGenesisExecutionResult> exe)
        {
            Text.White("\t"); Text.CliCommand(exe.CommandText, true); Text.White(" ("); Text.FriendlyText(exe.FriendlyName); Text.White(") found on "); Text.Blue(exe.GetType().Name); Text.Line();
        }
    }
}
