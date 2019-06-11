//using Genesis.Cli.Extensions;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Text;
//using System.Threading.Tasks;

//namespace Genesis.Cli.Commands
//{
//    public class GoCommand : GenesisCommand
//    {
//        public override string Name { get => "go"; }

//        public override string Description => "Execute the current configuration.";

//        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
//        {
//            genesis.ValidateGenesisContext();

//            return await genesis.ExecuteConfiguration();
//        }
//    }
//}
