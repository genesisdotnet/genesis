using Genesis.Cli.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class ClearCommand : GenesisCommand
    {
        public override string Name { get => "clear"; }

        public override string Description => "Clear the execution Chain";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            genesis.Objects.Clear();
            Text.GrayLine("Objects have been reset");

            Text.WhiteLine("Clearing the Chain");
            genesis.Chain.Clear();

            Console.Clear();

            return await Task.FromResult(new BlankTaskResult()); //because nothing broke and we have nothing to report. :| (uh)
        }
    }
}
