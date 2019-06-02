using Genesis.Cli.Extensions;
using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Genesis.Cli
{
    class Program
    {
        private static bool _isScript = false;
        private static string[] _script = new string[] { };
        private static Version _version;

        static async Task Main(string[] args)
        {
            InitializeVersion();

            //comment to run the loop
            //args = new string[] { "--script", "./LocalDBSqlToCSharp.genesis" };

            //NOTE:      --script "C:\Path\To\Script.genesis"
            //if (args.Length == 2 && args[0].ToLower() == "--script" && args[1].Length > 0)
                //await InitializeScript(args[1]);

            var tokenSource = new CancellationTokenSource(); //TODO: Is this even necessary here? 

            Console.WriteLine($"Genesis Creation Engine v{_version}"); //TODO: Get real version
            Console.WriteLine();

            await CommandLoader.InitAsync();

            if (!_isScript) //execute 'normally'
            {
                Console.WriteLine("HINT: '?' for assistance");
                do
                {
                    if (tokenSource.IsCancellationRequested)
                        Environment.Exit(-3);

                    Console.Write($@"genesis{GenesisContext.Scope?.PromptString}>");

                    args = Console.ReadLine().Split(" ");

                    ProcessCommandLine(args, tokenSource);
                }
                while (!tokenSource.IsCancellationRequested);
            }
            else //execute a script
            {
                Console.WriteLine($"Processing script '{args[1]}' with {_script.Length} lines");
                Console.WriteLine();
                foreach (var line in _script)
                {
                    Console.WriteLine($"Executing: '{line}' as {line.Split(" ").Length} arguments");

                    ProcessCommandLine(line.Split(" "), tokenSource);
                }
            }
        }

        private static void InitializeVersion()
        {
            _version = typeof(Program).Assembly.GetName().Version;
        }

        /// <summary>
        /// Expects a path to a .genesis file in order to execute it
        /// </summary>
        /// <param name="scriptPath">absolute or relative path to the .genesis script</param>
        /// <returns><see cref="Task"/></returns>
        private static async Task InitializeScript(string scriptPath)
        {
            if (!File.Exists(scriptPath))
            {
                Console.WriteLine("Invalid script path, quitting.");
                Environment.Exit(-1);
            }

            _script = await File.ReadAllLinesAsync(scriptPath);

            if (_script.Length == 0)
                throw new InvalidGenesisScriptException(scriptPath);

            for (var i = 0; i < _script.Length; i++)
                _script[i] = _script[i].Trim(); //cheesy strip off 

            _isScript = true;
            //TODO: Pre-parse or something to validate it?
        }

        /// <summary>
        /// Execute a single command according to the arguments passed in
        /// </summary>
        /// <param name="args"><see cref="string[]"/>line split up into arguments</param>
        /// <param name="tokenSource"><see cref="CancellationTokenSource"/>cancellation source</param>
        private static void ProcessCommandLine(string[] args, CancellationTokenSource tokenSource)
        {
            if (tokenSource.IsCancellationRequested)
                Environment.Exit(-2);

            CommandLineApplication app = ExecuteContext(args);

            try
            {
                if (0 == app.Execute(args)) //command executed successfully '0' is success, all others are errors
                {
                    Debug.WriteLine($@"Nice"); //TODO: Log more stuff, or at all :|
                }
                else //some way to get errors?
                {
                    Debug.WriteLine($@"Wtf?");
                }
            }
            catch (Exception cliex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($@"{cliex.Message}"); //displayed to the console
                Console.ResetColor();
            }
        }
        /// <summary>
        /// Build a new command line app and relay the args to it
        /// </summary>
        /// <param name="args">string array of arguments</param>
        /// <returns><see cref="CommandLineApplication"/>fresh cla</returns>
        private static CommandLineApplication ExecuteContext(string[] args)
        {
            var app = new CommandLineApplication(true) //doesn't matter if we rebuild it every execution since we have a context
            {
                Description = "Generate stuff from other stuff",
                //TODO: Default help stuff?
                ExtendedHelpText = "Populators create a pool of ObjectGraph objects.\nGenerators write out files according to what's in the ObjectGraphs\n If you want to support a 'thing' then just write a producer to produce an ObjectGraph. If you want to support 'something new', then write a Generator that consumes an ObjectGraph."
            };

            foreach (var cmd in CommandLoader.Commands)
            {
                app.Command(cmd.Name, cfg =>
                {
                    cfg.Description = cmd.Description;
                    cfg.HelpOption($"{cmd.HelpTemplate}");
                    cfg.Option("-?", "Display command details", CommandOptionType.SingleValue);
                    cfg.ShowInHelpText = true;
                    /* arguments and options */
                }, false) //false so it doesn't throw on unknown args, pop and gen commands have no way to know the args ahead of time
                .OnExecute((async () =>
                {
                    Genesis.ITaskResult result;
                    try
                    {
                        result = await cmd.Execute(GenesisContext.Current, args);
                        Console.WriteLine(result.Message);
                    }
                    catch (Exception exception)
                    {
                        result = new Genesis.BlankTaskResult();
                        Console.WriteLine(exception.Message);
                    }

                    return result.Success ? 0 : -1; //does this even matter?
                }));
            }

            return app;
        }
    }
}
