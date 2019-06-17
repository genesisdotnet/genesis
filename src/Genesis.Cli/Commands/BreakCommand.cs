using Genesis.Cli.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    class BreakCommand : GenesisCommand
    {
        public override string Name { get => "break"; }

        public override string Description => "Stop a script";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            //NOTE: This apparently can't affect anything on the Program class itself. (without some voodoo)
            //So, just dump some text and check for this command in Program. 
            Text.DarkYellowLine("Breaking...");
            Text.Yellow($"Dumping to a prompt, [");
            Text.CliCommand(GetType().Name.Replace("Command", ""));
            Text.YellowLine($"] was called.");
            Text.Line();

            return await Task.FromResult(new BlankTaskResult());
        }
    }
}
