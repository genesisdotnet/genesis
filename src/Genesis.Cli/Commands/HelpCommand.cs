using Genesis.Cli.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class HelpCommand : GenesisCommand
    {
        public override string Name { get => "help"; }
        public override string Description => "We'll try to";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("How this works:");
            Console.ResetColor();
            Console.Write("\tPopulators are called that because they 'populate' an ObjectGraph collection, usually from a data source. ");
            Console.WriteLine("Generators read from the ObjectGraph collection populators fill in, and output whatever is desired, typically writing a file. It could write emails, call a service, anything really though.");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Before anything can successfully execute, Populators and Generators have to be manually scanned / located with the 'scan' command. (for now)");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("Use '");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("?");
            Console.ResetColor();
            Console.WriteLine("' for a list of commands");

            return await Task.FromResult(new BlankTaskResult { Success = true, Message = "" }); ;
        }
    }
}
