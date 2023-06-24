/*
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using Serilog;

namespace MakItE.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            
                .UseSerilog((hostingContext, loggerConfiguration) => {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.FromLogContext();
                    //.Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
                    //.Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
                })
            
            
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
*/