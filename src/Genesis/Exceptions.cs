using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    public class InvalidGenesisScriptException : Exception
    {
        public InvalidGenesisScriptException(string scriptPath) : base($"Invalid script path '{scriptPath}'") { }
    }

    public class ExecutionAggregateException : AggregateException
    {
        public ExecutionAggregateException() : base("One or more exceptions have occured while executing the GenesisContext. See inner exceptions for details") { }
    }
}
