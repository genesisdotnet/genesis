using Genesis.Cli.Extensions;
using Genesis.Generation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class InspectCommand : GenesisCommand
    {
        public override string Name { get => "exec"; } //I keep typing 'exec' whatever, and it's annoying. 

        public override string Description => "Execute an IGenesisExecutor by alias (Populator or Generator)";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var btr = new BlankTaskResult();

            var exeName = args[1];

            //NOTE: Added that default help stuff for commands. Copy / Pasted 'gen' command here ;)

            //if (args.Length == 1 || HelpWasRequested(args)) //just 'gen' or 'gen --help,-?'
            //{
            //    //Todo: Initially wanted this here, not so sure now, maybe. 
            //}
            //else
            //{
            //    var generator = OutputManager.Generators.Find(g => g.CommandText.Trim().ToLower() == args[1].Trim().ToLower());
            //    if (generator != null)
            //    {
            //        await genesis.ConfigureGenerator(generator);
            //        Text.White($@"The current Generator is now: "); Text.CyanLine($"'{generator.GetType().Name}'");
            //        btr.Success = true;
            //        btr.Message = string.Empty;
            //    }
            //    else
            //    {
            //        Console.Write("'");
            //        Console.ForegroundColor = ConsoleColor.Yellow;
            //        Console.Write(args[1]);
            //        Console.ResetColor();
            //        Console.Write("'");
            //        Console.WriteLine(" is not a known Generator name.");

            //        btr.Message = "Invalid Generator.Name";
            //    }
            //}

            return await Task.FromResult(btr);
        }
    }
}
