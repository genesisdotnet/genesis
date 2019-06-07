using System.Composition;

namespace Genesis.Population
{
    /// <summary>
    /// Primary configuration class for individual Generators
    /// </summary>
    [Export(nameof(IPopulatorConfiguration), typeof(IPopulatorConfiguration))]
    public class PopulatorConfiguration : IPopulatorConfiguration
    {
        public PopulatorConfiguration()
        {
            
        }
    }
}