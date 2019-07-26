using Genesis.Cli.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class QuestionMarkCommand : GenesisCommand
    {
        public override string Name { get => "?"; }
        public override string Description => "Inception, display this information";
        public override string Usage => "?";
        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            await CommandLoader.InitAsync(args);

            Text.Line();
            Text.Cyan("Genesis"); Text.GrayLine($" {Program.GetVersionDisplayString()}");
            Text.Line();
            
            foreach(var cmd in CommandLoader.Commands)
            {
                if (cmd.Name == string.Empty) //default command leaves an empty line otherwise
                    continue;

                Text.Green($"\t{cmd.Name}"); Text.WhiteLine($"\t{cmd.Description}");
            }

            Text.Line();

            return await Task.FromResult(new BlankGenesisExecutionResult() { Success = true, Message = "" });
        }
    }
}
