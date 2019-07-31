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
using System.IO;

namespace Genesis.Cli.Commands
{
    public class StatusCommand : GenesisCommand
    {
        private static readonly string t = "\t";
        public override string Name { get => "status"; }
        public override string Description => "Display information about the configuration";

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            Text.Line();
            Text.WhiteLine("Known Inputs:");

            //TODO: Merge Inputs/Outputs & General Executors

            if (InputManager.Inputs.Count == 0)
            {
                Text.RedLine("\t no inputs found");
            }
            else
            {
                var tmp = new List<string>(args);

                if (ArgsContainDetailsFlag(tmp))
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

                if (ArgsContainDetailsFlag(tmp))
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

                if (ArgsContainDetailsFlag(tmp))
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

            Text.Line();
            Text.WhiteLine("ObjectGraph Contents:");
            foreach (var og in genesis.Objects)
            {
                Text.GreenLine("-=+<" + og.Name + ">+=-");

                Text.BlueLine(t + og.Name + ((og.KeyId != null && int.TryParse(og.KeyId.ToString(), out var o) && (int.Parse(og.KeyId.ToString())) > 0) ? " KeyId: " + og.KeyId : string.Empty)); //lame

                if(og.Properties.Count > 0)
                {
                    foreach (var p in og.Properties)
                    {
                        Text.Green(t + t + p.Name);
                        Text.White(" : ");
                        Text.FriendlyText(p.SourceType ?? string.Empty);
                        Text.Line();                        
                    }
                }

                if (og.Events.Count > 0)
                {
                    Text.YellowLine(t + "Events:");
                    foreach (var e in og.Events)
                        Text.DarkYellowLine(t + t + e.Name + ((!string.IsNullOrEmpty(e.DelegateFormattedName)) ? " - DelegateType: " + e.DelegateFormattedName : string.Empty));
                }

                if(og.Methods.Count > 0)
                {
                    Text.MagentaLine(t + "Methods:");
                    foreach (var m in og.Methods)
                    {
                        Text.DarkMagenta(t + t + m.ReturnTypeFormattedName);
                        Text.Yellow(t + t + m.Name);
                        m.WriteMethodSignature();
                    }
                }
            }

            Text.Line();

            Text.White("\t\t");

            genesis.WriteContextInfo();

            Text.Line();

            return await Task.FromResult(new BlankGenesisExecutionResult() { Success = true, Message = "" });
        }

        private static bool ArgsContainDetailsFlag(List<string> tmp) 
            => tmp.Contains("detail") // just accomodating common stuff
            || tmp.Contains("-d")
            || tmp.Contains("details")
            || tmp.Contains("detailed")
            || tmp.Contains("--detailed")
            || tmp.Contains("--detail")
            || tmp.Contains("--details")
            || tmp.Contains("--deets");

        private static void DisplayDetail(IGenesisExecutor<IGenesisExecutionResult> exe)
        {
            Text.White("\tCommand: "); Text.CliCommand(exe.CommandText, false); Text.Line();
            Text.White("\tSource: "); Text.Yellow(exe.GetType().Assembly.GetName().Name); Text.White("."); Text.BlueLine(exe.GetType().Name);
            Text.White("\t\t"); Text.FriendlyText(exe.FriendlyName, false); Text.Line();
            Text.White("\tFile: "); Text.DarkGrayLine(Path.GetFileName(exe.GetType().Assembly.Location));
            exe.DisplayConfiguration();
            Text.Line(); 
        }

        private static void DisplayQuick(IGenesisExecutor<IGenesisExecutionResult> exe)
        {
            Text.White("\t"); Text.CliCommand(exe.CommandText, true); Text.White(" ("); Text.FriendlyText(exe.FriendlyName); Text.White(") found on "); Text.Blue(exe.GetType().Name); Text.Line();
        }
    }
}
