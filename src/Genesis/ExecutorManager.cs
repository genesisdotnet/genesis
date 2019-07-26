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
using Genesis.Output;

namespace Genesis
{
    public static class ExecutorManager
    {
        public static List<IGenesisExecutor<IGenesisExecutionResult>> Current { get; set; } = new List<IGenesisExecutor<IGenesisExecutionResult>>();

        public static async Task InitializeExecutorsAsync(bool writeOutputMessages = false)
        {
            Current.Clear();

            Debug.WriteLine($@"Scanning local directory for OutputExecutor libraries");

            var assemblies = Directory.EnumerateFiles(Environment.CurrentDirectory, "*" + GenesisDefaults.LibraryExtension, SearchOption.TopDirectoryOnly)
                                        .Select(Assembly.LoadFile)
                                        .ToList();

            var conventions = new ConventionBuilder();

            conventions.ForTypesDerivedFrom<IGenesisExecutor<IGenesisExecutionResult>>()
                        .Export<IGenesisExecutor<IGenesisExecutionResult>>()
                        .Shared();

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies, conventions);

            using var container = configuration.CreateContainer();

            var executors = container.GetExports<IGenesisExecutor<IGenesisExecutionResult>>();

            Current.Clear();

            foreach (var executor in executors)
            {
                Current.Add(executor);

                try
                {
                    await executor.Initialize();

                    if (!writeOutputMessages)
                        continue;

                    Text.White($"'"); Text.Green(executor.CommandText); Text.White("' ("); Text.Cyan(executor.FriendlyName); Text.White(") was found in '"); Text.Blue(executor.GetType().Name); Text.White("'... ");
                    Text.GreenLine("OK");

                    Console.ResetColor();
                }
                catch (Exception exc)
                {
                    if (!writeOutputMessages)
                        continue;

                    Console.Write($"'{executor.FriendlyName}': ");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(exc.Message);
                    Console.ResetColor();
                }
            }
        }
    }
}
