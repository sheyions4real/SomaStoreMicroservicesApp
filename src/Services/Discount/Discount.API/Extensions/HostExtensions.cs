using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry=0)
        {
            // this method is to run the migrate database and will retry itself for 50 times after every two minutes
            // incase the discountdb database container
            // creation has not completed
            int retryForAvailability = retry.Value;

            using(var scope = host.Services.CreateScope()) // CreateScope {Dependency Injection} to use scope as reference as the main configuration in startup.cs
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>(); // to access the Configuration Settings via the services object
                var logger = services.GetRequiredService<ILogger<TContext>>();          // to access the Logger class which can be use to log information  {Console.log}

                try
                {
                    logger.LogInformation("Migrating Postgresssql Database");
                    // create posgress database connection

                    using var connection = new NpgsqlConnection(
                        configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

                    connection.Open();

                    using var command = new NpgsqlCommand 
                    { 
                        Connection = connection
                    };
                    // drop the table if it exist
                    command.CommandText = "DROP TABLE IF EXISTS Coupon";
                    command.ExecuteNonQuery();

                    // create the coupon table
                    command.CommandText = @"CREATE TABLE Coupon ( id SERIAL PRIMARY KEY, 
                        Productname VARCHAR(25) NOT NULL,
                        Description text,
                        Amount int)";
                    command.ExecuteNonQuery();


                    command.CommandText = "INSERT INTO Coupon(productname, description, amount)VALUES ('IPhone X', 'Iphone Discount Price', 250);";
                    command.ExecuteNonQuery();


                    command.CommandText = "INSERT INTO Coupon(productname, description, amount)VALUES ('Samsung s10', 'Samsung Discount Price', 350);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migrated Postgress Database.");
                    connection.Close();
                }
                catch (NpgsqlException ex)
                {
                    logger.LogInformation("An Error cooured while migrating the Postgress Database.");
                    logger.LogInformation("An Error ." + ex.Message.ToString());

                    if (retryForAvailability < 50)      // retry when an error occurs for 50 times after waiting for 2 minutes
                    {
                        retryForAvailability++;   // increase the number of retry count by 1
                        System.Threading.Thread.Sleep(2000); // Pause the Application for 2 minutes and then run the MigrateDatabase method again
                        MigrateDatabase<TContext>(host, retryForAvailability); // Recursive Function - Calling the Migrate database again to rerun the database scripts and see the database

                    }
                }


            } 
            
            return host;
        }

       
    }
}
