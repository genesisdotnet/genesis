using Genesis.Cli.Extensions;
using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Cli.Commands
{
    public class DefaultCommand : GenesisCommand
    {
        public override string Name => "";
        public override string Description => "";

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            return await Task.FromResult(new OutputTaskResult() { Success = true, Message = "Bad command or file name" }); //heh
        }
    }
}
