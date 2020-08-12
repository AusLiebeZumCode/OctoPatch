using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace OctoConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(b =>
                {
                    b.UseStartup<Startup>();
                });

            builder.Build().Run();
        }
    }
}
