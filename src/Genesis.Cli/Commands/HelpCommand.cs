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
            Text.Line();
            Text.DarkCyanLine("How this works:");
            Text.White("*");
            Text.YellowLine("\tDiscover Executor interfaces in the libraries that are right next to the cli. (for now)");
            Text.CliCommand("\tscan", false);Text.Line();
            Text.Yellow("\tAdd an Executor to the chain. "); Text.GrayLine("(probably an Input/Source/Origin)");
            Text.CliCommand("\tadd ", false); Text.CliCommand("mssql",false); Text.GrayLine();
            Text.Yellow("\tAdd another Executor. "); Text.GrayLine("(Output/Destination/Target)");
            Text.CliCommand("\tadd ", false); Text.CliCommand("poco", false); Text.GrayLine();
            Text.Yellow("\tExecute the chain sequentially. mssql populates ObjectGraphs, then poco reads them and writes out class files."); Text.GrayLine();
            Text.CliCommand("\texec chain ", false); Text.DarkGrayLine("\t\t\t\t(double check config for mssql.ConnectionString");
            Text.Line();
            Text.Line();
            Text.GrayLine("This effectively reads sql schema so that a poco generator can access that schema and do what it needs to. In this case, it's to write c# class files for each of the Sql Table objects.");

            return await Task.FromResult(new BlankTaskResult { Success = true, Message = "" }); ;
        }
    }
}
