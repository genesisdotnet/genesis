using System;
using System.IO;
using System.Threading.Tasks;

namespace Genesis.Input.Excel
{
    public class ExcelSheetInput : InputExecutor
    {
        public override string CommandText => "xl";
        public override string FriendlyName => "An .xlsx Sheet";
        public override string Description => "A Workbook or data from within an .xlsx (MS Excel) file";

        public ExcelSheetConfig Config { get; set; }

        protected override void OnInitialized(/*, string[] args */) //TODO: Pass args to the init 
        {
            Config = (ExcelSheetConfig)Configuration; //TODO: configuration is wonky
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            //TODO: Use some open source library to read from an .xls(x) file

            return await base.Execute(genesis, args);
        }
    }
}
