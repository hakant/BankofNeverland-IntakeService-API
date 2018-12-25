using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BankofNeverland.IntakeApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webHost = CreateWebHostBuilder(args).Build();
            await InitializeCosmosDb(webHost);
            webHost.Run();
        }

        private static async Task InitializeCosmosDb(IWebHost webHost)
        {
            var client = Resolve<IDocumentClient>(webHost);
            var cosmosDbConfig = Resolve<IOptions<CosmosDbConfig>>(webHost).Value;

            await client.CreateDatabaseIfNotExistsAsync(new Database { Id = cosmosDbConfig.DatabaseId });
            var databaseUri = UriFactory.CreateDatabaseUri(cosmosDbConfig.DatabaseId);

            await client.CreateDocumentCollectionIfNotExistsAsync(
                databaseUri,
                new DocumentCollection()
                {
                    Id = cosmosDbConfig.Collections.IntakesCollection
                });
        }

        private static T Resolve<T>(IWebHost webHost)
        {
            return (T)webHost.Services.GetService(typeof(T));
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
