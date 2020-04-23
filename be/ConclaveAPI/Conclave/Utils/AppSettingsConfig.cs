namespace Conclave.Utils
{
    /*
     * appsettings.json schema
     */
    public class AppSettingsConfig
    {
        public Storage Storage { get; set; } = new Storage();
        public Cache Cache { get; set; } = new Cache();
    }

    public class Storage
    {
        public string Uploads { get; set; }
    }
    public class Cache
    {
        public Redis Redis { get; set; }
    }
    public class Redis
    {
        public string Host { get; set; }
        public string Port { get; set; }
    }

}
