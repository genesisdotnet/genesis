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
    public class XVGenerator : OutputExecutor
    {
        public override string CommandText => "xv";
        public override string Description => "Generates a Xamarin.Forms View with fields for each property";
        public override string FriendlyName => "Xamarin.Forms Create/Edit View (XAML)";

        public XVConfig Config { get; set; }

        protected override void OnInitilized()
        {
            Config = (XVConfig)Configuration;
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
                                .Replace(Tokens.PropertiesStub, GetPropertiesReplacement(objectGraph.Properties));

            var subPath = Path.Combine(OutputPath, "Xamarin.Views"); //use cli to change this...

            if (!Directory.Exists(subPath))
                Directory.CreateDirectory(subPath);

            await Task.Delay(100); //timing or something weirdness

            File.WriteAllText(Path.Combine(subPath, objectGraph.Name.ToSingular() + ".cs"), output);
        }

        private string GetPropertiesReplacement(List<PropertyGraph> properties) //TODO: Figure out something for more configuration of the generators
        {
            string template = Environment.NewLine; //TODO: Properties for Xamarin Forms View XAML

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
    }
}
