using Microsoft.Extensions.Configuration;
using System.Composition;

namespace Genesis.Output
{
    /// <summary>
    /// Primary configuration class for individual Outputs
    /// </summary>
    [Export(nameof(IOutputConfiguration), typeof(IOutputConfiguration))]
    public class GeneratorConfiguration : IOutputConfiguration
    {
        public GeneratorConfiguration()
        {

        }
        public string Namespace { get; set; } = GenesisDefaults.Namespace;
        public string OutputPath { get; set; } = GenesisDefaults.OutputPath;
    }
}