using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
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
        }

    }
    public static class ToolsEx
    {
        private class Inputs
        {
            public dynamic input { get; internal set; }
        }
        public static Func<dynamic, string> ToFunc(this string script)
        {
            // https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples
            var lines = script.ToLines();
            var code = string.Join(";" + Environment.NewLine, lines);

            var csScript = CSharpScript.Create<string>(
                code,
                globalsType: typeof(Inputs)
                );

            var csDelegate = csScript.CreateDelegate();

           // csDelegate.run


            throw new NotImplementedException();
        }

        public static IEnumerable<string> ToLines(this string text)
        {
            using var reader = new StringReader(text);
            string line = null;
            while ((line = reader.ReadLine()) != null)
                yield return line;
        }

        public static IEnumerable<(string key, string script)> GetKeysAndScripts(this string filePath)
        {
            using var reader = new StreamReader(filePath);
            
            var key = string.Empty;
            StringBuilder? value = null;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine()?.Split(new[] { '|' }, 2).FirstOrDefault()?.Trim() ?? string.Empty;

                if (line.StartsWith(':') && line.EndsWith(':'))
                {
                    if (key.Length > 0)
                    {
                        yield return (key, value?.ToString() ?? string.Empty);
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
                    (value ?? (value = new StringBuilder())).Append(line).AppendLine();
                }
            }

            if (key != null)
            {
                yield return (key, value?.ToString());
            }
        }
    }
}
