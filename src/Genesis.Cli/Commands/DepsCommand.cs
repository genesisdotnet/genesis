using Genesis.Cli.Extensions;
using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class DepsCommand : GenesisCommand
    {
        public override string Name => "deps";
        public override string Description => "Manipulate generator dependencies";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var exe = (IOutputExecutor)GetExecutor(args[1]);
            switch (args[2])
            {
                case "list":
                    Text.Line();
                    foreach (var d in exe.Dependencies)
                    {
                        Text.YellowLine(d.PathFragment);
                        Text.White("\t");
                        Text.Blue(d.Contents.Length.ToString() + " bytes");
                        Text.Line();
                    }
                    break;
                case "dump":
                    await exe.DepositDependencies(exe.Configuration.OutputPath);
                    Text.Line();
                    break;
                default:
                    Text.Line();
                    Text.White("Choose an operation:");
                    Text.FriendlyText("\tdump", false); Text.WhiteLine("\tWrites dependencies to the filesystem.");
                    Text.FriendlyText("\tlist", false); Text.WhiteLine("\tDisplays dependency information");
                    Text.Line();
                    break;
            }

            return await Task.FromResult(new OutputTaskResult() { Success = true }); //heh
        }
    }
}
