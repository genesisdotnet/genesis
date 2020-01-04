using Genesis.Output;
using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.ApiServiceConfig
{
    public class AspNetServiceConfig : GeneratorConfiguration
    {
        public string DtoNamespace { get; set; } = "Genesis.Common";
        public string ModelNamespace { get; set; } = "Genesis.Data.Models";
        public string MapperNamespace { get; set; } = "Genesis.Services.Mapping";
        public string RepoNamespace { get; set; } = "Genesis.Data.Repositories";
        public string CachedRepoNamespace { get; set; } = "Genesis.Data.CachedRepositories";
        public string ModelBaseClass { get; set; } = "Model";
        public string ObjectBaseClass { get; set; } = "Service";
        public string DtoSuffix { get; set; } = "Dto";
        public string MapperSuffix { get; set; } = "Mapper";
        public string RepoSuffix { get; set; } = "Repository";
        public string CachedRepoSuffix { get; set; } = "CachedRepository";
        public string Language { get; set; } = "C#";
        public string Overwrite { get; set; } = "all";
        public string Preserve { get; set; } = "none";
        public bool GenericBaseClass { get; set; } = true;
    }
}