using Discount.Grpc.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc
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

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
