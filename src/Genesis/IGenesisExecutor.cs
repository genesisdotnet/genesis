using Genesis;
using System.Threading.Tasks;

namespace Genesis
{
    public interface IGenesisExecutor<TGenesisExecutionResult> where TGenesisExecutionResult : IGenesisExecutionResult
    {
        string CommandText { get; }
        string Description { get; }
        string FriendlyName { get; }

        bool Initialized { get; }
        Task Initialize();
        Task DisplayConfiguration();
        Task<bool> EditConfig<TPropertyType>(string propertyName, TPropertyType value);
        Task<TGenesisExecutionResult> Execute(GenesisContext genesis, string[] args);
    }
}
