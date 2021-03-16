using System;

namespace Genesis.Input.MySqlDb
{
    public class MySqlConfig : InputConfiguration
    {
        public string Server { get; set; } = "";
        public string Database {get;set;} = "";
        public string UserId { get; set; } = "";
        public string Password { get; set; } = "";
        public bool ConvertZeroDateTime { get; set; } = true;
        public string ToConnectionString()
            => $"Server={Server};Database={Database};UID={UserId};PWD={Password};ConvertZeroDateTime={ConvertZeroDateTime};";

        public string[] ExcludePrefixes { get; set; } = Array.Empty<string>();
    }
}