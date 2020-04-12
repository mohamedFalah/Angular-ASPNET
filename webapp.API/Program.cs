using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using webapp.API.Data;

namespace webapp.API
{
    public class Program
    {
        //run the everything: not exists app doesnt run
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using(var scope = host.Services.CreateScope()){
                var Services = scope.ServiceProvider;
                try
                {
                    var context = Services.GetRequiredService<DataContext>();
                    context.Database.Migrate();
                    Seed.SeedUsers(context);   
                }
                catch (Exception ex)
                { 
                    var logger = Services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex,"an error occured during migration");
                }
            }

            host.Run();

        }

        //tell program to use something
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //we can some configuration to the app <startup>
                    webBuilder.UseStartup<Startup>();
                });
    }
}
