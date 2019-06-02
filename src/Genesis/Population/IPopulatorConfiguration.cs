using Microsoft.Extensions.Configuration;

namespace Genesis.Population
{
    public interface IPopulatorConfiguration
    {
        string ConfigurationString { get; set; }
    }
}