using Genesis.Cli;
using System.Threading.Tasks;

namespace Genesis
{
    //TODO: Rename, think its the same as a task method
    public interface IGenesisExecutor<IGenesisExecutionResult> 
    {
        string CommandText { get; }
        string Description { get; }
        string FriendlyName { get; }

        bool Initialized { get; }
        Task Initialize();

        Task<bool> EditConfig<TPropertyType>(string propertyName, TPropertyType value);
        Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string args);
    }
}