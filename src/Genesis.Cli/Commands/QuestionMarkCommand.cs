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
        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            await CommandLoader.InitAsync(args);

            Text.WhiteLine();
            Text.Cyan("Genesis"); Text.WhiteLine($" {Program.GetVersionDisplayString()}");
            foreach(var cmd in CommandLoader.Commands)
            {
                Text.Green($"\t{cmd.Name}"); Text.WhiteLine($"\t\t{cmd.Description}");
            }
            return await Task.FromResult(new BlankTaskResult() { Success = true, Message = "" });
        }
    }
}
