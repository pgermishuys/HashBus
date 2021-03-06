﻿namespace HashBus.WebApi
{
    using System;
    using System.Configuration;

    class Program
    {
        static void Main()
        {
            var baseUri = new Uri(ConfigurationManager.AppSettings["BaseUri"]);
            var mongoConnectionString = ConfigurationManager.AppSettings["MongoConnectionString"];
            var mongoDBDatabase = ConfigurationManager.AppSettings["MongoDBDatabase"];

            App.Run(baseUri, mongoConnectionString, mongoDBDatabase);
        }
    }
}
