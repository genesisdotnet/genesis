using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Output.Protos
{
    public class ProtoServiceGenerator : OutputExecutor
    {
        public override string CommandText => "proto-svc";
        public override string Description => "Generates a service class implementation of a protobuf";
        public override string FriendlyName => "Protobuf Implementation Service Generator";

        public ProtoServiceConfig Config { get; set; }

        protected override void OnInitialized()
        {
            Config = (ProtoServiceConfig)Configuration;
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputGenesisExecutionResult();

            await DepositDependencies();

            try
            {
                foreach (var obj in genesis.Objects)
                {
                    await ExecuteGraph(obj);
                }
                result.Success = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                result.Message = e.Message;
            }

            return result;
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            var tmp = Template;

            if (!Directory.Exists(Config.OutputPath))
                Directory.CreateDirectory(Config.OutputPath);

            var output = tmp.Raw.Replace(Tokens.Version, Config.Version.ToString())
                                .Replace(Tokens.ObjectName, objectGraph.Name.ToSingular())
                                .Replace(Tokens.MethodsStub, GetMethodsReplacement(objectGraph.Methods))
                                .Replace(Tokens.Namespace, Config.Namespace)
                                .Replace(Tokens.KeyDataType, objectGraph.KeyDataType.ToCodeDataType())
                                .Replace(Tokens.GrpcNamespace, Config.GrpcNamespace)
                                .Replace(Tokens.DepsServiceNamespace, Config.DepsServiceNamespace)
                                .Replace(Tokens.DepsDtoNamespace, Config.DepsDtoNamespace)
                                .Replace(Tokens.ServiceSuffix, Config.ServiceSuffix)
                                .Replace(Tokens.Injections, GrpcServiceInjector.GetParameterForServiceClass(objectGraph, Config.ServiceSuffix))
                                .Replace(Tokens.InjectionMembers, GrpcServiceInjector.GetDeclarationForServiceClass(objectGraph, Config.ServiceSuffix, true))
                                .Replace(Tokens.InjectionAssignment, GrpcServiceInjector.GetAssignmentForServiceClass(objectGraph))
                                ;

            var outPath = Path.Combine(Config.OutputPath, objectGraph.Name.ToSingular() + Config.ServiceSuffix + ".cs");

            File.WriteAllText(outPath, output);

            Text.White($"Wrote '"); Text.Yellow(outPath); Text.WhiteLine("'");

            await Task.CompletedTask;
        }

        private static string GetMethodsReplacement(List<MethodGraph> methods)
        {
            var rows = new List<string>();
            var sb = new StringBuilder();
            foreach (var m in methods)
            {
                sb.Clear();
                m.Parameters.ForEach(p => sb.Append(p.DataTypeFormattedName + " " + p.Name.ToCorrectedCase() + ","));
                var s = sb.ToString();
                var tmp = s.Length > 0 ? s[..^1] : s;

                rows.Add(@$"\t\tpublic override {m.ReturnTypeFormattedName} {m.Name}({tmp})\n\t\t{{\n\t\t\t\\\\\throw new NotImplementedException();n\t\t}}");
            }

            return string.Join(Environment.NewLine, rows.ToArray());
        }

        private static string GetPropertiesReplacement(IEnumerable<PropertyGraph> properties)
        {
            string template = "\t\t~PROPERTY_DATATYPE~ ~PROPERTY_MEMBER_NAME~ = ~COUNTER~;";

            var sb = new StringBuilder();

            var count = 1;
            foreach (var p in properties)
            {
                if (p.SourceType.ToLower() == "sysname") //TODO: Exclusion list on each input executor? 
                {
                    //TODO: display something in the UI?
                    continue;
                }

                if (!p.SourceType.Equals(GenesisDefaults.UnknownSourceType, StringComparison.Ordinal))
                    sb.AppendLine(template.Replace(Tokens.PropertyDataType, p.SourceType.ToBasicDataType())
                        .Replace(Tokens.PropertyName, p.Name)
                        .Replace(Tokens.PropertyCounter, count++.ToString())
                        .Replace(Tokens.PropertyMemberName, p.Name.ToCorrectedCase()));
            }

            return sb.ToString();
        }
    }
}
