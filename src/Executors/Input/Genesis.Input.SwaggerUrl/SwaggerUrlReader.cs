using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Namotion.Reflection;
using NJsonSchema.CodeGeneration.CSharp;
using NSwag;
using NSwag.CodeGeneration;
using NSwag.CodeGeneration.CSharp;
using TypeInfo = System.Reflection.TypeInfo;

namespace Genesis.Input.SwaggerUrl
{
    public class SwaggerUrlReader : InputExecutor
    {
        private static readonly Regex _nullableRegex = new Regex("(?<=<)[^>]*(?=>)"); // all text between <>, could be improved I'm sure
        
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

        private static readonly string[] _usings = {
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
            // grab the .json from the url and create an OpenApiDocument from it
            var apiDoc = await DownloadOpenApiDocument(); 

            var csgen = CreateCSharpGenerator(apiDoc);

            var comp = await CreateCSharpCompilation(csgen, Enum.Parse<LanguageVersion>(Config.LanguageVersion));

            // Dump the compilation to a memory stream
            var (stream, result) = EmitCompilationToStream(comp);

            if(!result.Success)
                return WriteFailureDetails(result); // bail on fail

            // create the temp assembly so we can reflect over it. 
            var (tmpAss, ctx) = GenerateTemporaryAssembly(stream);

            foreach(var c in tmpAss.GetTypes().Where(w=>w.IsClass))
            {
                if (c.IsReflectionClosure()) // skip the reflection only types
                    continue;

                var (cls, obj) = CreateObjectGraph(c, Config.OutputNamespace);

                CreateObjectPropertyGraphs(cls, obj);

                CreateObjectMethodGraphs(genesis, cls, obj);
            }

            UnloadAssembly(ctx);

            await stream.DisposeAsync();

            Text.SuccessGraffiti();

            return await base.Execute(genesis, args); //TODO: fix the whole IGenesisExecutionResult "stuff"
        }

        private static void UnloadAssembly(WeakReference ctx)
        {
            Text.White("Unloading temporary assembly... ");

            ctx.UnloadAssembly( /*waits for it to unload*/);

            Text.GreenLine("OK");
        }

        private static void CreateObjectMethodGraphs(GenesisContext genesis, TypeInfo cls, ObjectGraph obj)
        {
            var events = new List<string>();

            foreach (var m in cls.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (m.Name.StartsWith("get_") || m.Name.StartsWith("set_"))
                    continue; //already did property accessors

                if (m.Name.StartsWith("add_") || m.Name.StartsWith("remove_"))
                {
                    var name = m.Name.Split('_')[1]; //just get the event Name
                    if (!events.Contains(name))
                    {
                        events.Add(name);
                    }
                }

                Debug.WriteLine($@"Method:{(_nullableRegex.IsMatch(m.Name) ? _nullableRegex.Match(m.Name).Value : m.Name)}");
                var methodGraph = new MethodGraph
                {
                    Name = _nullableRegex.IsMatch(m.Name) ? _nullableRegex.Match(m.Name).Value : m.Name,
                    MethodVisibility = MethodVisibilities.Public,
                    ReturnDataType = m.ReturnType,
                    ReturnTypeFormattedName = m.ReturnType.GetFormattedName(),
                    HasGenericParams = m.ContainsGenericParameters,
                    IsGeneric = m.IsGenericMethod,
                    FormattedGenericArguments = m.GetGenericArguments().ToFormattedNames(),
                };

                foreach (var par in m.GetParameters().OrderBy(o => o.Position))
                {
                    var mp = new ParameterGraph
                    {
                        DataType = par.ParameterType,
                        DataTypeFormattedName = par.ParameterType.GetFormattedName(),
                        DisplayName = par.ParameterType.GetDisplayName(),
                        Name = par.Name,
                        IsOut = par.IsOut,
                        IsIn = par.IsIn,
                        IsOptional = par.IsOptional,
                        Position = par.Position,
                        IsGeneric = par.ParameterType.IsGenericType,
                        IsGenericMethodParameter = par.ParameterType.IsGenericMethodParameter,
                        GenericArgumentFormattedTypeNames = par.ParameterType.GetGenericArguments().ToFormattedNames(),
                    };

                    methodGraph.Parameters.Add(mp);
                }

                obj.Methods.Add(methodGraph);

                Text.GrayLine(
                    $"\tMethod: {m.Name}, Return: {methodGraph.ReturnTypeFormattedName}, Visibility: {(m.IsPublic ? "public" : m.IsPrivate ? "private" : "protected")}");
            }

            genesis.Objects.Add(obj);
        }

        private static void CreateObjectPropertyGraphs(TypeInfo cls, ObjectGraph obj)
        {
            foreach (var p in cls.GetProperties().Where(w => w.MemberType == MemberTypes.Property))
            {
                var pg = new PropertyGraph
                {
                    Name = _nullableRegex.IsMatch(p.Name) ? _nullableRegex.Match(p.Name).Value : p.Name,
                    SourceType = p.PropertyType.GetFormattedName(true),
                    IsKeyProperty = p.Name.EndsWith("Id", StringComparison.CurrentCultureIgnoreCase), //NOTE: cheesy
                };

                foreach (var m in p.GetAccessors())
                {
                    if (m.Name.StartsWith("get_"))
                        pg.GetterVisibility = (m.IsPublic) ? MethodVisibilities.Public :
                            m.IsPrivate ? MethodVisibilities.Private : MethodVisibilities.Protected;

                    if (m.Name.StartsWith("set_"))
                        pg.SetterVisibility = (m.IsPublic) ? MethodVisibilities.Public :
                            m.IsPrivate ? MethodVisibilities.Private : MethodVisibilities.Protected;
                }

                obj.Properties.Add(pg);
                Text.GrayLine(
                    $"\tProperty: {p.Name}, Type: {p.PropertyType.GetFormattedName()}, Get: {Enum.GetName(typeof(MethodVisibilities), pg.GetterVisibility)}, Set: {Enum.GetName(typeof(MethodVisibilities), pg.SetterVisibility)}");
            }
        }

        private static (TypeInfo cls, ObjectGraph obj) CreateObjectGraph(Type c, string ns)
        {
            var cls = c.GetTypeInfo();

            var obj = new ObjectGraph
            {
                Name = cls.GetFormattedName(), //ext method to parse for Generics
                Namespace = ns,
                BaseType = typeof(object) == cls.BaseType ? null : cls.BaseType,
                BaseTypeFormattedName = cls.BaseType?.GetFormattedName()
            };

            if (cls.IsGenericType)
            {
                var genericArgs = cls.GetGenericArguments().ToFormattedNames();
                obj.IsGeneric = true;
                obj.GenericArgumentTypes = genericArgs;
            }

            Text.GrayLine(
                $"Class: {cls.Name}, Base:{obj.BaseTypeFormattedName}, Generic:{cls.IsGenericType}, Access:{(cls.IsPublic ? cls.IsVisible ? "public" : "internal" : "private")}");
            return (cls, obj);
        }

        private static (Assembly tempAsm, WeakReference ctx) GenerateTemporaryAssembly(MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var tmpAss = GenesisAssembly.FromStream(stream, out var ctx);
            return (tmpAss, ctx);
        }

        private static IGenesisExecutionResult WriteFailureDetails(EmitResult result)
        {
            Text.RedLine("Unable to build temp library");
            var failures = result.Diagnostics.Where(diagnostic =>
                diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var diagnostic in failures)
                Text.RedLine($@"\t{diagnostic.Id}: {diagnostic.GetMessage()}");

            return new InputGenesisExecutionResult {Success = false, Message = "Errors occurred"};
        }

        private static (MemoryStream stream, EmitResult result) EmitCompilationToStream(CSharpCompilation comp)
        {
            var stream = new MemoryStream();

            Text.White("Creating temporary objects... ");
            var result = comp.Emit(stream);
            Text.GreenLine("OK");
            return (stream, result);
        }

        private static async Task<CSharpCompilation> CreateCSharpCompilation(CSharpClientGenerator csgen, LanguageVersion version = LanguageVersion.CSharp8)
        {
            Text.White($"Processing contents... ");
            var code = csgen.GenerateFile(ClientGeneratorOutputType.Full);
            Text.GreenLine("OK");

            Text.WhiteLine($"Determining dependencies");

            var refs = GenesisGlobals.TrustedAssembliesPaths
                .Where(p => neededLibs.Contains(Path.GetFileNameWithoutExtension(p)))
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();

            refs.Add(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location));

            var options = new CSharpParseOptions(version);

            Text.WhiteLine("Defining entry point");
            using var rdr = new StringReader(code);

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
            //File.WriteAllText(@"C:\Temp\GeneratedCode.cs", code);

            Text.WhiteLine("Creating Syntax Tree");
            var syntax = CSharpSyntaxTree.ParseText(code, options: options);

            Text.WhiteLine("Preprocessing");
            var comp = CSharpCompilation.Create("swag-gen-temp.dll", new[] {syntax}, refs.ToArray());
            return comp;
        }

        private CSharpClientGenerator CreateCSharpGenerator(OpenApiDocument apiDoc)
        {
            var settings = new CSharpClientGeneratorSettings
            {
                GenerateDtoTypes = true,
                AdditionalContractNamespaceUsages = _usings,
                AdditionalNamespaceUsages = _usings
            };
            settings.CodeGeneratorSettings.InlineNamedAny = true;
            settings.CSharpGeneratorSettings.Namespace = Config.OutputNamespace;
            settings.CSharpGeneratorSettings.ClassStyle = CSharpClassStyle.Inpc;

            var generator =
                new NSwag.CodeGeneration.OperationNameGenerators.
                    MultipleClientsFromFirstTagAndPathSegmentsOperationNameGenerator();

            var csgen = new CSharpClientGenerator(apiDoc, settings);
            csgen.Settings.GenerateOptionalParameters = true;
            csgen.Settings.GenerateResponseClasses = true;
            csgen.Settings.SerializeTypeInformation = true;
            csgen.Settings.OperationNameGenerator = generator;
            return csgen;
        }

        private async Task<OpenApiDocument> DownloadOpenApiDocument()
        {
            Text.WhiteLine($"Downloading from [{Config.Address}]");

            //https://swagger.io/docs/specification/basic-structure/
            var gen = await OpenApiDocument.FromUrlAsync(Config.Address);

            gen.GenerateOperationIds();
            return gen;
        }
    }
}
