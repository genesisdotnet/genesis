using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Genesis.Generation
{
    public static class DependencyManager
    {
        public static List<IGenesisDependency> LoadDependencies(IGenerator executor)
        {
            var result = new List<IGenesisDependency>();

            var fileName = executor.GetType().Name + GenesisDefaults.DependenciesExtension;

            string t = string.Empty;
            if (File.Exists(fileName))
                t = File.ReadAllText(fileName);

            var tmp = new string[] { Tokens.TemplateSeperator };
            var chunks = t.Split(tmp, StringSplitOptions.RemoveEmptyEntries);

            foreach (var i in chunks)
            {
                using (var rdr = new StringReader(i))
                {
                    var pathFragment = rdr.ReadLine();
                    var content = rdr.ReadToEnd();

                    result.Add(new GenesisDependency(pathFragment, content));

                    rdr.Dispose();
                }
            }

            return result;
        }
    }
}
