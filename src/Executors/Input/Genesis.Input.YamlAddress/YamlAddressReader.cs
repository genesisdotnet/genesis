using Genesis;
using Genesis.Input;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Genesis.Input.YamlAddress
{
    public class YamlAddressReader : InputExecutor
    {
        public override string CommandText => "yaml";
        public override string FriendlyName => "YAML Endpoint";
        public override string Description => "A YAML source via address";

        public YamlConfig Config { get; set; }

        protected override void OnInitilized(/*, string[] args */) //TODO: Pass args to the init 
        {
            Config = (YamlConfig)Configuration; //TODO: configuration is wonky
        }

        public override async Task<ITaskResult> Execute(GenesisContext genesis, string[] args)
        {
            //TODO: Pull a yaml string off the web and parse it into object graphs representing the schema

            var net = new HttpClient();
            var yamlString = await net.GetStringAsync(Config.Address);
            //Text.DarkYellowLine(yamlString);

            //https://swagger.io/docs/specification/basic-structure/

            var obj = new ObjectGraph();
            obj.Properties.Add(new PropertyGraph {
                Name = "StringProperty",
                GraphType = GraphTypes.Property,
                Accesibility = "public",
                IsNullable = false,
                TypeGuess = "string",
                SourceType = "srctypeString"
            });
            obj.Properties.Add(new PropertyGraph
            {
                Name = "NumberProperty",
                GraphType = GraphTypes.Property,
                Accesibility = "public",
                IsNullable = false,
                TypeGuess = "int",
                SourceType = "srctypeInt"
            });
            obj.Properties.Add(new PropertyGraph
            {
                Name = "BoolProperty",
                GraphType = GraphTypes.Property,
                Accesibility = "public",
                IsNullable = false,
                TypeGuess = "bool",
                SourceType = "srctypeBool"
            });

            await genesis.AddObject(obj); //for now... more concerned with adding outputs

            return await base.Execute(genesis, args); //TODO: fix the whole ITaskResult "stuff"
        }
    }
}
