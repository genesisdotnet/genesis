using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration;
using NSwag.CodeGeneration.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Genesis;
using NJsonSchema.Infrastructure;

namespace Genesis.Input.SwaggerUrl
{
    public class SwaggerUrlReader : InputExecutor
    {
        private static Regex _nullableRegex = new Regex("(?<=<)[^>]*(?=>)");

        public override string CommandText => "swag";
        public override string FriendlyName => "Swagger Endpoint";
        public override string Description => "Swagger data via URL";

        public SwagConfig Config { get; set; }

        private static readonly string[] neededLibs = {
            "mscorlib",
            "netstandard",
            "System.Core",
            "System.Runtime",
            "System.IO",
            "System.ObjectModel",
            "System.Linq",
            "System.Net.Http",
            "System.Collections",
            "System.CodeDom.Compiler",
            "System.ComponentModel",
            "System.ComponentModel.Annotations",
            "System.Net.Primitives",
            "System.Runtime.Serialization",
            "System.Runtime.Serialization.Primitives",
            "System.Runtime.Extensions",
            "System.Private.Uri",
            "System.CodeDom",
            "System.Composition.AttributedModel",
            "System.Composition.Convention",
            "System.Composition.Runtime",
            "System.Diagnostics.Tools",
            "Microsoft.CodeAnalysis.CSharp",
            "NJsonSchema",
            "Newtonsoft.Json",
        };

        // kinda lame for now
        private const string ENTRY_POINT = @"
        public static int Main(string[] args)
        {
            return 0;
        }
        ";

        protected override void OnInitialized(/*, string[] args */) //TODO: Pass args to the init 
        {
            Config = (SwagConfig)Configuration; //TODO: configuration is wonky
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            Text.WhiteLine($"Downloading from [{Config.Address}]");

            //https://swagger.io/docs/specification/basic-structure/
            var gen = await OpenApiDocument.FromUrlAsync(Config.Address);

            gen.GenerateOperationIds();
            
            var usings = new[] {
                "System",
                "System.Text",
                "System.Collections",
                "System.Collections.Generic",
                "System.Threading.Tasks",
                "System.Linq",
                "System.Net.Http",
                "System.ComponentModel.DataAnnotations",
                "Newtonsoft.Json",
            };

            var settings = new CSharpClientGeneratorSettings
            {
                GenerateDtoTypes = true,
                AdditionalContractNamespaceUsages = usings,
                AdditionalNamespaceUsages = usings
            };
            settings.CodeGeneratorSettings.InlineNamedAny = true;
            settings.CSharpGeneratorSettings.Namespace = Config.OutputNamespace;
            settings.CSharpGeneratorSettings.ClassStyle = CSharpClassStyle.Inpc;

            var generator = new NSwag.CodeGeneration.OperationNameGenerators.MultipleClientsFromFirstTagAndPathSegmentsOperationNameGenerator();
            
            var csgen = new CSharpClientGenerator(gen, settings);
            csgen.Settings.GenerateOptionalParameters = true;
            csgen.Settings.GenerateResponseClasses = true;
            csgen.Settings.SerializeTypeInformation = true;
            csgen.Settings.OperationNameGenerator = generator;

            Text.White($"Processing contents... ");
            var code = csgen.GenerateFile(ClientGeneratorOutputType.Full);
            
            Text.GreenLine("OK");

            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            

            Text.WhiteLine($"Determining dependencies");

            var refs = trustedAssembliesPaths
                        .Where(p => neededLibs.Contains(Path.GetFileNameWithoutExtension(p)))
                        .Select(p => MetadataReference.CreateFromFile(p))
                        .ToList();

            refs.Add(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location));

            var options = new CSharpParseOptions(LanguageVersion.CSharp8);

            Text.WhiteLine("Defining entry point");
            var rdr = new StringReader(code);
            var lines = new StringBuilder();
            var i = 0;
            string line;
            while ((line = await rdr.ReadLineAsync()) != null)
            {
                lines.AppendLine(line);
                if (i == 26) //lol lame
                    lines.AppendLine(ENTRY_POINT);
                i++;
            }
       
            code = lines.ToString();
            File.WriteAllText(@"C:\Temp\GeneratedCode.cs", code);

            Text.WhiteLine("Creating Syntax Tree");
            var syntax = CSharpSyntaxTree.ParseText(code, options: options);

            Text.WhiteLine("Preprocessing");
            var comp = CSharpCompilation.Create("swag-gen-temp.dll", new[] { syntax }, refs.ToArray());
            
            await using var stream = new MemoryStream();

            Text.White("Creating temporary objects... ");
            var result = comp.Emit(stream);
            Text.GreenLine("OK");

            if(!result.Success)
            {
                Text.RedLine("Unable to build temp library");
                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures)
                    Text.RedLine($@"\t{diagnostic.Id}: {diagnostic.GetMessage()}");
              
                return new InputGenesisExecutionResult { Success = false, Message = "Errors occurred" };
            }

            stream.Seek(0, SeekOrigin.Begin);

            var tmpAss = AssemblyLoadContext.Default.LoadFromStream(stream);
            
            // loop classes
            foreach(var c in tmpAss.GetTypes().Where(w=>w.IsClass))
            {
                var cls = c.GetTypeInfo();
                if (cls.IsGenericType)
                {
                    
                }

                var obj = new ObjectGraph {
                    Name = cls.GetFormattedName(), //ext method to parse for Generics
                    Namespace = Config.OutputNamespace,
                    GraphType = GraphTypes.Object,
                    BaseType = cls.BaseType,
                    BaseTypeFormattedName = cls.BaseType?.GetFormattedName()
                };

                Text.GrayLine($"Class: {cls.Name}, Base:{obj.BaseTypeFormattedName}, Generic:{cls.IsGenericType}, Access:{(cls.IsPublic ? cls.IsVisible ? "public" : "internal" : "private")}");

                foreach (var p in cls.GetProperties().Where(w=>w.MemberType == MemberTypes.Property))
                {
                    var pg = new PropertyGraph {
                        Name = p.Name,
                        SourceType = p.PropertyType.GetFormattedName(),
                        IsKeyProperty = p.Name.EndsWith("Id", StringComparison.CurrentCultureIgnoreCase), //NOTE: cheesy
                    };

                    foreach (var m in p.GetAccessors())
                    {
                        if (m.Name.StartsWith("get_"))
                            pg.GetterVisibility = (m.IsPublic) ? MethodVisibilities.Public: m.IsPrivate ? MethodVisibilities.Private : MethodVisibilities.Protected;
                        
                        if (m.Name.StartsWith("set_"))
                            pg.SetterVisibility = (m.IsPublic) ? MethodVisibilities.Public : m.IsPrivate ? MethodVisibilities.Private : MethodVisibilities.Protected;
                    }
                    obj.Properties.Add(pg);
                    Text.GrayLine($"\tProperty: {p.Name}, Type: {p.PropertyType.GetFormattedName()}, Get: {Enum.GetName(typeof(MethodVisibilities), pg.GetterVisibility)}, Set: {Enum.GetName(typeof(MethodVisibilities), pg.SetterVisibility)}");
                }

                foreach (var m in cls.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    var meth = new MethodGraph
                    {
                        Name = m.Name.Replace("get_", string.Empty),
                        MethodVisibility = MethodVisibilities.Public,
                        ReturnDataType = m.ReturnType,
                        ReturnTypeFormattedName = m.ReturnType.GetFormattedName(),
                        HasGenericParams = m.ContainsGenericParameters,
                        IsGeneric = m.IsGenericMethod,
                    };

                    foreach (var par in m.GetParameters().OrderBy(o=>o.Position))
                    {
                        var mp = new ParameterGraph {
                            DataType = par.ParameterType,
                            DataTypeFormattedName = par.ParameterType.GetFormattedName(),
                            Name = par.Name,
                            IsOut = par.IsOut,
                            IsIn = par.IsIn,
                            IsOptional = par.IsOptional,
                            Position = par.Position
                        };
                        
                        meth.Parameters.Add(mp);
                    }

                    Text.GrayLine($"\tMethod: {m.Name}, Return: {meth.ReturnTypeFormattedName}, Visibility: {(m.IsPublic ? "public" : m.IsPrivate ? "private" : "protected")}");
                }
                genesis.Objects.Add(obj);
            }

            Text.SuccessGraffiti();

            return await base.Execute(genesis, args); //TODO: fix the whole IGenesisExecutionResult "stuff"
        }
    }
}
