using Genesis.Output;
using System.Composition;

namespace Genesis.Executors
{
    [Export(nameof(IGeneralConfiguration), typeof(IGeneralConfiguration))]
    public class GraphToolsConfig : IGeneralConfiguration
    {
        public string OutputPath { get; set; } = @"Output\";
    }
}
