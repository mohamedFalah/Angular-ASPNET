using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace webapp.API
{
    public class Program
    {
        //run the everything: not exists app doesnt run
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
