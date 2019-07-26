using Genesis;
using Genesis.Output.Templates;
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

namespace Genesis.Executors
{
    public static class GeneralManager
    {
        public static List<IGeneralExecutor> Current { get; set; } = new List<IGeneralExecutor>();

        public static async Task InitializeGeneratorsAsync(bool writeOutputMessages = false)
        {
            Current.Clear();

            Debug.WriteLine($@"Scanning local directory for OutputExecutor libraries");

            var assemblies = new List<Assembly>();

            foreach (var file in Directory.EnumerateFiles(Environment.CurrentDirectory, "*" + GenesisDefaults.LibraryExtension, SearchOption.TopDirectoryOnly))
                assemblies.Add(Assembly.LoadFile(file)); //doesn't seem to mind loading everything... for now

            var conventions = new ConventionBuilder();

            conventions.ForTypesDerivedFrom<IGeneralExecutor>()
                        .Export<IGeneralExecutor>()
                        .Shared();

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies, conventions);

            using var container = configuration.CreateContainer();

            var generators = container.GetExports<IGeneralExecutor>();

            Current.Clear();

            foreach (var generator in generators)
            {
                Current.Add(generator);

                try
                {
                    await generator.Initialize();

                    if (!writeOutputMessages)
                        continue;

                    Text.White($"'"); Text.Green(generator.CommandText); Text.White("' ("); Text.Cyan(generator.FriendlyName); Text.White(") was found in '"); Text.Blue(generator.GetType().Name); Text.White("'... ");
                    Text.GreenLine("OK");

                    Console.ResetColor();
                }
                catch (Exception exc)
                {
                    if (!writeOutputMessages)
                        continue;

                    Text.CliCommand(generator.CommandText); Text.White(": ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(exc.Message);
                    Console.ResetColor();
                }
            }
        }
    }
}
