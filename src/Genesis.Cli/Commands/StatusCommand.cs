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
    public class StatusCommand : GenesisCommand
    {
        public override string Name { get => "status"; }
        public override string Description => "Display information about the configuration";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("Known Populators:");

            if (InputManager.Populators.Count == 0)
            {
                Text.RedLine("\t no populators found");
            }
            else
            {
                foreach (var pop in InputManager.Populators)
                {
                    //TODO: More info here?
                    Text.White($"\t'"); Text.Green(pop.CommandText); Text.White($"' is defined in '"); Text.Cyan($"{pop.FriendlyName}"); Text.WhiteLine("'");

                }
            }

            Console.ResetColor();
            Console.WriteLine("Known Generators:");

            if (OutputManager.Generators.Count == 0)
            {
                Text.RedLine("\t no generators found");
            }
            else
            {
                foreach (var gen in OutputManager.Generators)
                {
                    //TODO: More info here?
                    Text.White($"\t'"); Text.Green(gen.CommandText); Text.White($"' is defined in '"); Text.Cyan($"{gen.FriendlyName}"); Text.WhiteLine("'");
                }
            }

            Console.ResetColor();

            genesis.WriteContextInfo();

            return await Task.FromResult(new BlankTaskResult() { Success = true, Message = "" });
        }
    }
}
