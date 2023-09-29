using Genesis.Input;

namespace Genesis.Input.MSSqlDb
{
    public class SqlConfig : InputConfiguration
    {
        public string ConnectionString { get; set; } = "Server=localhost;User=sa;Password=1234qwer";
        public string[] ExcludePrefixes { get; set; } = System.Array.Empty<string>();
    }
}