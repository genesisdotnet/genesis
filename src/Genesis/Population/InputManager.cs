using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Population
{
    public class InputManager
    {
        public static List<IPopulator> Populators { get; set; } = new List<IPopulator>();

        /// <summary>
        /// Load Populator extensions from the current directory
        /// </summary>
        public static async Task InitializePopulatorsAsync(bool writeOutputMessages = false)
        {
            Populators.Clear();

            Debug.WriteLine($@"Scanning local directory for Populator libraries");

            var assemblies = new List<Assembly>();

            foreach (var file in Directory.EnumerateFiles(Environment.CurrentDirectory, "*" + GenesisDefaults.LibraryExtension, SearchOption.TopDirectoryOnly))
                assemblies.Add(Assembly.LoadFile(file));

            var conventions = new ConventionBuilder();

            conventions.ForTypesDerivedFrom<IPopulator>()
                        .Export<IPopulator>()
                        .Shared();

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies, conventions);

            //TODO: Post-build steps for Populator assemblies

            using (var container = configuration.CreateContainer())
            {
                var populators = container.GetExports<IPopulator>();

                Populators.Clear();

                foreach (var populator in populators)
                {
                    Populators.Add(populator);

                    var configType = populator.GetType().GetRuntimeProperty("Config").PropertyType ?? typeof(PopulatorConfiguration);
                    var config = configType.IsInstanceOfType(typeof(PopulatorConfiguration));
                    var cfgWarning = false;
                    try
                    {
                        if (typeof(PopulatorConfiguration).IsAssignableFrom(configType)) //Make sure we can use it
                            populator.Configuration = (IPopulatorConfiguration)Activator.CreateInstance(configType, true);

                        //bind the configuration json to the config instance
                        if (writeOutputMessages && !GenesisConfiguration.ApplyFromJson(configType, $"{configType.Name}.json", populator.Configuration))
                        {
                            cfgWarning = true;
                            Text.RedLine($"Unable to configure from {configType.Name}.json for {populator.GetType().Name}");
                        }

                        await populator.Initialize();

                        if (!writeOutputMessages)
                            continue;

                        Console.Write($"'{populator.FriendlyName}': ");
                        Console.ForegroundColor = (cfgWarning) ? ConsoleColor.Yellow : ConsoleColor.Green;
                        Console.WriteLine((cfgWarning) ? "Warning" : "Success");
                        Console.ResetColor();
                    }
                    catch (Exception exc)
                    {
                        if (!writeOutputMessages)
                            continue;

                        Console.Write($"'{populator.FriendlyName}': ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(exc.Message);
                        Console.ResetColor();
                    }
                }
            }
        }
    }
}
