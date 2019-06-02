using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Genesis
{
    public class GenesisConfiguration : IGenesisConfiguration
    {
        public static bool ApplyFromJson(Type configType, string filePath, object instance = null)
        {
            filePath = filePath ?? throw new ArgumentNullException(nameof(filePath), "The full path with filename at the end. (Must exist)");
            instance = instance ?? throw new ArgumentNullException(nameof(instance), "This is the instance of the object you want the configuration values mapped to");

            bool result;

            if (File.Exists(filePath))
            {
                var fileName = Path.GetFileName(filePath);

                Debug.WriteLine($"Configuration found ({fileName}) for '{configType.FullName}'.\n\tApplying to class from [{filePath}]");
                var cfg = new ConfigurationBuilder() //TODO: Make configuration less brittle
                                .SetBasePath(Environment.CurrentDirectory)
                                .AddJsonFile(filePath)
                                .Build();
                cfg.Bind(instance); //well that was handy
                result = true;
            }
            else
            {
                result = false;
                Debug.WriteLine($"Configuration .json NOT FOUND for {configType.Name}.json for configuration type {configType.FullName}\n\tat [{filePath}]");
            }
            return result;
        }
    }
}
