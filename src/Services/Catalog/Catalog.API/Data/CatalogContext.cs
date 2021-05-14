using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data
{
    public class CatalogContext : ICatalogContext
    {

        public CatalogContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString")); // to create connection to the mongo database
            var database = client.GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName")); // if the database on does not exist it will create it

            Products = database.GetCollection<Product>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));     // get the collection name from the database object
            CatalogContextSeed.SeedData(Products);  // check if there is a product in the collection otherwise add some productsto the Collection Passed to it  
        }   

        public IMongoCollection<Product> Products { get; }
    }
}
