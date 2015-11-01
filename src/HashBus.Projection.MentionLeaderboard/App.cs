using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HashBus.ReadModel;
using HashBus.ReadModel.RavenDB;
using NServiceBus;
using Raven.Client.Document;

namespace HashBus.Projection.UserLeaderboard
{
    class App
    {
        public static async Task RunAsync(string ravenDBUrl, string ravenDBDatabase)
        {
            using (var store = new DocumentStore())
            {
                store.Url = ravenDBUrl;
                store.DefaultDatabase = ravenDBDatabase;
                store.EnlistInDistributedTransactions = false;
                store.Initialize();

                var busConfiguration = new BusConfiguration();
                busConfiguration.EndpointName("HashBus.Projection.MentionLeaderboard");
                busConfiguration.UseSerialization<JsonSerializer>();
                busConfiguration.EnableInstallers();
                busConfiguration.UsePersistence<InMemoryPersistence>();
                busConfiguration.SendFailedMessagesTo("error");
                busConfiguration.LimitMessageProcessingConcurrencyTo(1);
                busConfiguration.RegisterComponents(c =>
                    c.RegisterSingleton<IRepository<string, IEnumerable<Mention>>>(
                        new RavenDBListRepository<Mention>(store)));

                using (await Bus.Create(busConfiguration).StartAsync())
                {
                    Thread.Sleep(Timeout.Infinite);
                }
            }
        }
    }
}
