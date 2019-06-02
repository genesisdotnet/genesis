using Genesis;
using Genesis.Cli;
using Genesis.Generation.Templates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Composition;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Generation
{
    public static class OutputManager
    {
        public static List<IGenerator> Generators { get; set; } = new List<IGenerator>();

        /// <summary>
        /// Load Generator extensions from the current directory
        /// </summary>
        public static async Task InitializeGeneratorsAsync(bool writeOutputMessages = false)
        {
            Generators.Clear();

            Debug.WriteLine($@"Scanning local directory for Generator libraries");

            var assemblies = new List<Assembly>();

            foreach (var file in Directory.EnumerateFiles(Environment.CurrentDirectory, "*" + GenesisDefaults.LibraryExtension, SearchOption.TopDirectoryOnly))
                assemblies.Add(Assembly.LoadFile(file)); //doesn't seem to mind loading everything... for now

            var conventions = new ConventionBuilder();

            conventions.ForTypesDerivedFrom<IGenerator>()
                        .Export<IGenerator>()
                        .Shared();

            var configuration = new ContainerConfiguration().WithAssemblies(assemblies, conventions);

            using (var container = configuration.CreateContainer())
            {
                var generators = container.GetExports<IGenerator>();

                Generators.Clear();

                foreach (var generator in generators)
                {
                    Generators.Add(generator);

                    var configType = generator.GetType().GetRuntimeProperty("Config").PropertyType ?? typeof(GeneratorConfiguration);
                    var config = configType.IsInstanceOfType(typeof(GeneratorConfiguration));
                    var cfgWarning = false;
                    try
                    {
                        if (typeof(GeneratorConfiguration).IsAssignableFrom(configType)) //is this even necessary since ^ IsInstanceOf
                            generator.Configuration = (IGeneratorConfiguration)Activator.CreateInstance(configType, true);

                        //bind the configuration json to the config instance
                        if (writeOutputMessages && !GenesisConfiguration.ApplyFromJson(configType, $"{configType.Name}.json", generator.Configuration))
                        {
                            cfgWarning = true;
                            Text.RedLine($"Unable to configure from {configType.Name}.json for {generator.GetType().Name}");
                        }

                        //load a template - don't like it throwing exceptions
                        try
                        {
                            var template = TemplateLoader.LoadTemplateFor(generator); //brittle?, every generator must have a template to be valid during scan

                            generator.Template = template; //whatever for now
                        }
                        catch (Exception)
                        {
                            Text.DarkYellow("Problems loading template for: '"); Text.Cyan(generator.FriendlyName); Text.DarkYellow("' (Generator)");
                            //throw; //maybe it doesn't need a template
                        }
                        
                        await generator.Initialize();

                        if (!writeOutputMessages)
                            continue;

                        Console.Write($"'{generator.FriendlyName}': ");
                        Console.ForegroundColor = (cfgWarning) ? ConsoleColor.Yellow : ConsoleColor.Green;
                        Console.WriteLine((cfgWarning) ? "Warning" : "Success");
                        Console.ResetColor();
                    }
                    catch (Exception exc)
                    {
                        if (!writeOutputMessages)
                            continue;

                        Console.Write($"'{generator.FriendlyName}': ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(exc.Message);
                        Console.ResetColor();
                    }

                    try
                    {
                        
                    }
                    catch(Exception ex)
                    {
                        Text.DarkYellowLine($"Couldn't load template: '{ex.Message}'");
                    }
                }
            }
        }
    }
}
