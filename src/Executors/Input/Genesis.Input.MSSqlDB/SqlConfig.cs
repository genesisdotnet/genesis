using Genesis.Input;

namespace Genesis.Input.MSSqlDb
{
    public class SqlConfig : InputConfiguration
    {
        public string ConnectionString { get; set; } = "Server=localhost;User=sa;Password=";
    }
}