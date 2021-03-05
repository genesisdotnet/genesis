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

namespace Genesis.Input
{
    public class InputManager
    {
        public static List<IInputExecutor> Inputs { get; set; } = new List<IInputExecutor>();

        //TODO: Why is InputManager 'writing' messages to the Console?

        /// <summary>
        /// Load InputExecutor extensions from the current directory
        /// </summary>
        //slow down son
        public static async Task InitializeInputsAsync(bool writeOutputMessages = false)
        {
            Inputs.Clear();

            var assemblies = new List<Assembly>();

            foreach (var file in Directory.EnumerateFiles(Environment.CurrentDirectory, "*" + GenesisDefaults.LibraryExtension, SearchOption.TopDirectoryOnly))
                assemblies.Add(Assembly.LoadFile(file));

            var conventions = new ConventionBuilder();

            conventions.ForTypesDerivedFrom<IInputExecutor>()
                        .Export<IInputExecutor>()
                        .Shared();

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies, conventions);

            //TODO: Post-build steps for InputExecutor assemblies

            using var container = configuration.CreateContainer();
            var populators = container.GetExports<IInputExecutor>();

            Inputs.Clear();

            foreach (var populator in populators)
            {
                Inputs.Add(populator);

                var configType = populator.GetType().GetRuntimeProperty("Config").PropertyType ?? typeof(InputConfiguration);
                var cfgWarning = false;
                try
                {
                    //cmdtext was located from friendlyname and is available if it succeedes
                    if (typeof(InputConfiguration).IsAssignableFrom(configType)) //Make sure we can use it
                        populator.Configuration = (IInputConfiguration)Activator.CreateInstance(configType, true);

                    //bind the configuration json to the config instance
                    if (writeOutputMessages && !GenesisConfiguration.ApplyFromJson(configType, $"{configType.Name}.json", populator.Configuration))
                    {
                        cfgWarning = true;
                        Text.RedLine($"Unable to configure from {configType.Name}.json for {populator.GetType().Name}");
                    }

                    await populator.Initialize();

                    if (!writeOutputMessages)
                        continue;

                    Text.White($"'"); Text.Green(populator.CommandText); Text.White("' ("); Text.Cyan(populator.FriendlyName); Text.White(") was found in '"); Text.Blue(populator.GetType().Name); Text.White("'... ");

                    if (cfgWarning)
                        Text.YellowLine("warning");
                    else
                        Text.GreenLine("OK");

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
