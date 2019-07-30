using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Runtime.CompilerServices;
using System.Text;

namespace Genesis
{
    /// <summary>
    /// Load and Unload assemblies
    /// </summary>
    public class GenesisAssemblyLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public GenesisAssemblyLoadContext(string path = "./") : base(true) // true means it's collectible, i.e. able to unload assemblies
        {
            _resolver = new AssemblyDependencyResolver(path);
        }

        protected override Assembly Load(AssemblyName name)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(name);
            return assemblyPath != null 
                ? LoadFromAssemblyPath(assemblyPath) 
                : null;
        }
    }
}
