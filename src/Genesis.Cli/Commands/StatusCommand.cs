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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\t no populators found");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;

                foreach (var pop in InputManager.Populators)
                {
                    //TODO: More info here?
                    Text.White($"\t'"); Text.Green(pop.CommandText); Text.WhiteLine($"' is defined in {pop.GetType().Name}");
                }
            }

            Console.ResetColor();

            Console.WriteLine("Known Generators:");
            
            if(OutputManager.Generators.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\t no generators found");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Cyan;

                foreach (var gen in OutputManager.Generators)
                {
                    //TODO: More info here?
                    Text.White($"\t'");Text.Green(gen.CommandText);Text.WhiteLine($"' is defined in {gen.GetType().Name}");
                }
            }
            
            Console.ResetColor();

            genesis.WriteContextInfo();

            return await Task.FromResult(new BlankTaskResult() { Success = true, Message = "" });
        }
    }
}
