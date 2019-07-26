using System;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Cli.Extensions
{
    public interface IGenesisCommand
    {
        string Name { get; }
        string HelpTemplate { get; }
        string Description { get; }
        string Usage { get; }

        Task InitializeAsync(string[] args);

        Task ProcessHelpCommand(string[] args);

        Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args);
    }
}