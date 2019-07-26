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
using System.Threading.Tasks;
using NJsonSchema.Infrastructure;

namespace Genesis.Input.SwaggerUrl
{
    public class SwaggerUrlReader : InputExecutor
    {
        public override string CommandText => "swag";
        public override string FriendlyName => "Swagger Endpoint";
        public override string Description => "Swagger data via URL";

        public SwagConfig Config { get; set; }

        // kinda lame for now
        private const string ENTRY_POINT = @"
        public static int Main(string[] args)
        {
            return 0;
        }
        ";

        protected override void OnInitilized(/*, string[] args */) //TODO: Pass args to the init 
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
            var neededLibs = new [] {
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

            Text.WhiteLine($"Determining dependencies");

            var refs = trustedAssembliesPaths
                        .Where(p => neededLibs.Contains(Path.GetFileNameWithoutExtension(p)))
                        .Select(p => MetadataReference.CreateFromFile(p))
                        .ToList();

            refs.Add(MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location));

            var options = new CSharpParseOptions(LanguageVersion.CSharp8, DocumentationMode.Parse, SourceCodeKind.Regular);

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
                Text.GrayLine($"Class: {c.Name}");

                var obj = new ObjectGraph {
                    Name = c.Name,
                    Namespace = Config.OutputNamespace,
                    GraphType = GraphTypes.Object,
                };

                foreach(var p in c.GetProperties().Where(w=>w.MemberType == MemberTypes.Property)) {

                    if (!p.CanWrite || !p.GetSetMethod( /*nonPublic*/ true).IsPublic)
                        continue; //for now;

                    var pg = new PropertyGraph {
                        Name = p.Name,
                        SourceType = p.PropertyType.Name,
                        IsKeyProperty = p.Name.EndsWith("Id", StringComparison.CurrentCultureIgnoreCase), //NOTE: cheesy
                        Accesibility = (p.CanWrite 
                                        && p.GetSetMethod(/*nonPublic*/ true).IsPublic) 
                                        ? "public"
                                        : "protected"
                    };

                    obj.Properties.Add(pg);
                    Text.GrayLine($"\tProperty: {c.Name}");
                }

                foreach (var m in c.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    var meth = new MethodGraph
                    {
                        Name = m.Name.Replace("get_", string.Empty),
                        MethodVisibility = MethodVisibilities.Public,
                        ReturnDataType = m.ReturnType,
                        HasGenericParams = m.ContainsGenericParameters,
                        IsGeneric = m.IsGenericMethod,
                    
                    };

                    foreach (var par in m.GetParameters().Where(w=>w.IsIn).OrderBy(o=>o.Position))
                    {
                        var mp = new ParameterGraph {
                            DataType = par.ParameterType,
                            Name = par.Name,
                            IsOut = par.IsOut,
                            IsOptional = par.IsOptional,
                            Position = par.Position
                        };
                        
                        meth.Parameters.Add(mp);

                        Text.GrayLine($"\tMethod: {c.Name}");
                    }
                }
                genesis.Objects.Add(obj);
            }

            Text.SuccessGraffiti();

            return await base.Execute(genesis, args); //TODO: fix the whole IGenesisExecutionResult "stuff"
        }
    }
}
