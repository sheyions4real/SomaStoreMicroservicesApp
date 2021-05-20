using Discount.API.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // default CreateHostBuilder that build the Host Application {server}
            // this will be overriden with out new host to run the migrate database class on startup
            //CreateHostBuilder(args).Build().Run();

            var host = CreateHostBuilder(args).Build();

            host.MigrateDatabase<Program>(); // the host will call the MigrateDatabase class to migrate the database before it runs the application

            host.Run(); // After the MigrateDatabase completes the host can now run the application
           

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
