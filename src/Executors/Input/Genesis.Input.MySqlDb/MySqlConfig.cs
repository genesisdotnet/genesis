using Genesis.Input;

namespace Genesis.Input.MySqlDb
{
    public class MySqlConfig : InputConfiguration
    {
        public string Server { get; set; } = "";
        public string Database {get;set;} = "";
        public string UserId { get; set; } = "";
        public string Password { get; set; } = "";

        public string ToConnectionString()
            => $"Server={Server}; Database={Database}; Uid={UserId}; Pwd={Password}";

        public string[] ExcludePrefixes { get; set; } = System.Array.Empty<string>();
    }
}