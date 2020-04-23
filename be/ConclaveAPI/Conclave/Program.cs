using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Reflection;

namespace Conclave
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            Directory.SetCurrentDirectory(rootPath);

            var host = new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();

            host.Run();
        }
    }
}
