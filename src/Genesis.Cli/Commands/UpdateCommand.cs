using Genesis.Cli.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class UpdateCommand : GenesisCommand
    {
        public override string Name { get => "update"; }
        public override string Description => "Eventually download latest and restart";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            return await Task.FromResult(new BlankTaskResult());
        }
    }
}
