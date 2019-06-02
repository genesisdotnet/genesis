using System;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Cli.Extensions
{
    public interface IGenesisCommand
    {
        string Name { get; }
        string HelpTemplate { get; }
        Task InitializeAsync(/*init params?*/);
        string Description { get; }
        string Usage { get; }
        
        Task<ITaskResult> Execute(GenesisContext genesis, string[] args);
    }
}