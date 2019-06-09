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
            Text.DarkGrayLine("Obsolete...");
            Text.White($"Clearing Populator: {genesis.Populator.GetType().Name} (");
            Text.Green(genesis.Populator.CommandText);
            genesis.Populator = null;
            Text.WhiteLine($")");

            ///////////////////////
            //don't forget what's loaded
            genesis.Objects.Clear();
            Text.GrayLine("Objects have been reset");

            Text.DarkGrayLine("Obsolete...");
            Text.White($"Clearing Generator: {genesis.Generator.GetType().Name} (");
            Text.Green(genesis.Generator.CommandText);
            genesis.Generator = null;
            Text.WhiteLine($")");

            Text.WhiteLine("Clearing the Chain");
            genesis.Chain.Clear();

            return await Task.FromResult(new BlankTaskResult()); //because nothing broke and we have nothing to report. :| (uh)
        }
    }
}
