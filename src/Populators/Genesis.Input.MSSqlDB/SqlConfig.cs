using Genesis.Population;

namespace Genesis.Input.MSSqlDb
{
    public class SqlConfig : PopulatorConfiguration
    {
        public string ConnectionString { get; set; } = "Server=localhost;User=sa;Password=";
    }
}