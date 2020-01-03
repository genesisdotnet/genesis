using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.CachedRepo
{
    public class RepoConfig : GeneratorConfiguration
    {
        public string ObjectBaseClass { get; set; } = "IRepository";
        public string ModelBaseClass { get; set; } = "Model";
        public bool GenericBaseClass { get; set; } = true;
        public string DepsModelNamespace { get; set; } = "Genesis.Data.Models";
        public string DepsRepoNamespace { get; set; } = "Genesis.Data.Repositories";
        public string DepsDbContext { get; set; } = "AppDbContext";
    }
}
