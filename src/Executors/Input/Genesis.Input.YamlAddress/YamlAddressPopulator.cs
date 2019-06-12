using Genesis;
using Genesis.Input;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Genesis.Input.YamlAddress
{
    public class YamlAddressInput : InputExecutor
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
            var net = new HttpClient();
            var yamlString = await net.GetStringAsync(Config.Address);
            Text.DarkYellowLine(yamlString);

            return await base.Execute(genesis, args);
        }
    }
}
