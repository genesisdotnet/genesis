using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Genesis;
using Genesis.Output;

namespace Genesis.Output.Poco
{
    public class XvmGenerator : OutputExecutor
    {
        public override string CommandText => "xvm";
        public override string Description => "Generates a basic ViewModel with property changed event / base";
        public override string FriendlyName => "Xamarin.Forms CRUD ViewModel";

        public XvmConfig Config { get; set; }

        protected override void OnInitilized()
        {
            Config = (XvmConfig)Configuration;
        }

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            var result = new OutputTaskResult();

            foreach (var obj in genesis.Objects)
            {
                await ExecuteGraph(obj);
            }

            return result;
        }

        public async Task ExecuteGraph(ObjectGraph objectGraph)
        {
            var tmp = this.Template;

            if (Directory.Exists(OutputPath))
                Directory.CreateDirectory(OutputPath);

            Thread.Sleep(100); //below fails saying directory doesn't exist... even though ^ runs fine?!


            //TODO: Output types are still sourcetypes, not poco types

            var output = tmp.Raw.Replace(Tokens.Namespace, Config.Namespace)    //TODO: Templating engine? / razor etc would be cool ..|., T4 
                                .Replace(Tokens.ObjectName, objectGraph.Name.ToSingular())
                                .Replace(Tokens.PropertiesStub, GetPropertiesReplacement(objectGraph.Properties))
                                .Replace(Tokens.ConstructionStub, GetConstructionReplacement(objectGraph.Properties));

            var subPath = Path.Combine(OutputPath, "XamarinViewModels");

            if (!Directory.Exists(subPath))
                Directory.CreateDirectory(subPath);

            await Task.Delay(100); //timing or something weirdness

            File.WriteAllText(Path.Combine(subPath, objectGraph.Name.ToSingular() + ".cs"), output);
        }

        private string GetPropertiesReplacement(List<PropertyGraph> properties) //TODO: Figure out something for more configuration of the generators
        {
            string template = "\t\tprivate ~PROPERTY_DATATYPE~ ~PROPERTY_MEMBER_NAME~;" + Environment.NewLine +     //TODO: This sucks
                                "\t\tpublic ~PROPERTY_DATATYPE~ ~PROPERTY_NAME~" + Environment.NewLine +
                                "\t\t{" + Environment.NewLine +
                                "\t\t\tget => ~PROPERTY_MEMBER_NAME~;" + Environment.NewLine +
                                "\t\t\tset => ~PROPERTY_MEMBER_NAME~; //Change property generator for xam viewmodels" + Environment.NewLine +
                                "\t\t}" + Environment.NewLine;

            var sb = new StringBuilder();

            foreach (var p in properties)
            {
                if (p.SourceType != "UNKNOWN")
                    sb.AppendLine(template.Replace(Tokens.PropertyDataType, p.SourceType)
                        .Replace(Tokens.PropertyName, p.Name)
                        .Replace(Tokens.PropertyMemberName, p.Name.ToCorrectedCase()));
            }

            return sb.ToString();
        }

        private string GetConstructionReplacement(List<PropertyGraph> properties)
        {
            const string template = "            //this.~PROPERTY_MEMBER_NAME~ = default(~PROPERTY_DATATYPE~);";

            var sb = new StringBuilder();

            foreach (var p in properties)
            {
                if (p.SourceType == "sysname")
                    continue;

                sb.AppendLine(template.Replace(Tokens.PropertyMemberName, p.Name.ToCorrectedCase())
                                    .Replace(Tokens.PropertyDataType, p.SourceType.ToCodeDataType()));
            }

            return sb.ToString();
        }
    }
}
