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
            var files = from f in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.KeyDef")
                        select (f, Parse(filePath: f));

            var values = files.ToArray();
        }

        private static IEnumerable<(string, string)> Parse(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                string key = null;
                StringBuilder value = null;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine()?.Split(new[] { '|' }, 2).FirstOrDefault()?.Trim();

                    if (line.StartsWith(':') && line.EndsWith(':'))
                    {
                        if (key != null)
                        {
                            yield return (key, value?.ToString());
                            value = null;
                            key = null;
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
}
