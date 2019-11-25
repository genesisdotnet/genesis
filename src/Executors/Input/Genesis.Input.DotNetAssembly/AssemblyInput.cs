using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

namespace Genesis.Input.DotNetAssembly
{
    public class AssemblyInput : InputExecutor
    {
        public override string CommandText => "netlib";
        public override string FriendlyName => ".Net Assembly";
        public override string Description => "Create graph entries from public classes in a .Net core assembly";

        public AssemblyInputConfig Config { get; set; } = new AssemblyInputConfig();

        protected override void OnInitialized(/*, string[] args */) //TODO: Pass args to the init 
        {
            Config = (AssemblyInputConfig)Configuration;
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args) 
        {
            var result = new BlankGenesisExecutionResult();

            var alc = new AssemblyLoadContext("DotNetAssembly", true);

            using var stream = File.OpenRead(Config.AssemblyPath);

            var asm = alc.LoadFromStream(stream);

            if (Config.OnlyGenesisDecorations)
            {
                foreach(var t in asm.DefinedTypes.Where(w => // pull objects with GenesisObject attribute
                    w.CustomAttributes.SingleOrDefault(ca => ca.AttributeType == typeof(GenesisObjectAttribute)) != null))
                {
                    InsertGraphFromType(genesis, t);
                }

            }
            else // use all public objects
            {
                foreach (var t in asm.DefinedTypes)
                { 
                    InsertGraphFromType(genesis, t); 
                }
            }
        
            return await Task.FromResult(result);
        }

        private void InsertGraphFromType(GenesisContext genesis, TypeInfo type)
        {
            var og = new ObjectGraph { 
                Name = type.Name,
                BaseType = type.BaseType!,
            };
            
            foreach(var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                og.Properties.Add(new PropertyGraph
                {
                    Name = p.Name,
                    SourceType = p.PropertyType.Name,
                    GetterVisibility = (p.IsPublic("get"))
                                            ? MethodVisibilities.Public
                                            : MethodVisibilities.Private, //TODO: change to MethodInfo.IsPublic...
                    SetterVisibility = (p.IsPublic("set"))
                                            ? MethodVisibilities.Public
                                            : MethodVisibilities.Private,
                });
            }
            genesis.Objects.Add(og);
        }
    }
}
