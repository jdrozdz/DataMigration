namespace DataMigration.Models
{
    public class ConfigModel
    {
        public Config SourceConfig { get; set; }
        public Config HostConfig { get; set; }
    }

    public class Config
    {
        public string ConnectionString { get; set; }
    }
}
