using Microsoft.Extensions.Configuration;
using System.Composition;

namespace Genesis.Generation
{
    /// <summary>
    /// Primary configuration class for individual Outputs
    /// </summary>
    [Export(nameof(IGeneratorConfiguration), typeof(IGeneratorConfiguration))]
    public class GeneratorConfiguration : IGeneratorConfiguration
    {
        public GeneratorConfiguration()
        {

        }
        public string Namespace { get; set; } = GenesisDefaults.Namespace;
        public string OutputPath { get; set; } = GenesisDefaults.OutputPath;
    }
}