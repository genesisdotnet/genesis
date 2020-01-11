using Genesis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;

namespace IdeaBucket
{
    class Program
    {
        static void Main(string[] args)
        {
            var codes = from f in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.KeyDef")
                        from s in f.GetKeysAndScripts()
                        select (
                        type: Path.GetFileNameWithoutExtension(f),
                        key: s.key,
                        script: s.script,
                        operation: s.script.ToFunc()
                        );

            var values = codes.ToArray();

            foreach (var v in values)
            {
                Console.WriteLine(v);
                Console.WriteLine($@"\t{ v.operation(new
                {
                })}");
            }
        }

    }
    public static class Inputs
    {
        private class Input
        {
            public dynamic Raw { get; internal set; } = new ExpandoObject();
        }

        public static Func<dynamic, string> ToFunc(this string script)
        {
            // https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples

            /*
             (1,1): error CS0656: Missing compiler required member 'Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create'
             */

            var lines = script.ToLines();
            var code = string.Join(";" + Environment.NewLine, lines);

            var csScript = CSharpScript.Create<string>(
                code,
                globalsType: typeof(Input),
                options: ScriptOptions.Default
                                      .WithReferences(typeof(Input).Assembly)
                                      .AddReferences("Microsoft.CSharp")

                );
            //NOTE: This should be Func<GenesisContext, ObjectGraph, string>
            //return new Func<GenesisContext, ObjectGraph, string>(obj => csScript.CreateDelegate().Invoke(obj));

            return new Func<dynamic, string>(obj => csScript.CreateDelegate().Invoke(obj));
        }

        public static IEnumerable<string> ToLines(this string text)
        {
            using var reader = new StringReader(text);
            string? line = null;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }

        public static IEnumerable<(string key, string? script)> GetKeysAndScripts(this string filePath)
        {
            using var reader = new StreamReader(filePath);
            
            string? key = null;
            StringBuilder? value = null;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine()?.Split(new[] { '|' }, 2).FirstOrDefault()?.Trim() ?? "";
                if (line.StartsWith(':') && line.EndsWith(':'))
                {
                    if (key?.Length > 0)
                    {
                        yield return (key, value?.ToString());
                        value = null;
                    }
                    key = line.Trim(':');
                }
                else if (string.IsNullOrWhiteSpace(line))
                {
                    //Ignore me
                }
                else
                {
                    if (value != null) value.AppendLine();
                    (value ?? (value = new StringBuilder())).Append(line);
                }
            }

            if (key != null)
            {
                yield return (key, value?.ToString() ?? string.Empty);
            }
        }
    }
}
