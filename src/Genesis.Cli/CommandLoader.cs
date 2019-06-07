using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Genesis.Cli.Extensions;

namespace Genesis.Cli
{
    public class CommandLoader
    {
        public static List<IGenesisCommand> Commands { get; } = new List<IGenesisCommand>();

        public static async Task InitAsync(string[] args)
        {
            var assemblies = new List<Assembly>();

            foreach (var file in Directory.EnumerateFiles(Environment.CurrentDirectory, "*" + GenesisDefaults.LibraryExtension, SearchOption.TopDirectoryOnly))
                assemblies.Add(Assembly.LoadFile(file));

            var conventions = new ConventionBuilder();

            conventions.ForTypesDerivedFrom<IGenesisCommand>()
                        .Export<IGenesisCommand>()
                        .Shared();

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies, conventions);

            using var container = configuration.CreateContainer();
            var commands = container.GetExports<IGenesisCommand>();

            Commands.Clear();

            foreach (var command in commands)
            {
                try
                {
                    Commands.Add(command);
                    await command.InitializeAsync(args); //raises all the commands' OnInitializing(args)
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($@"Boo, init failed for {command.GetType().Name}. {ex.Message}. It may or may not work...");
                    Console.ResetColor();
                }
            }
        }
    }
}