using System.Threading.Tasks;
using HashBus.ReadModel;
using HashBus.ReadModel.RavenDB;
using Raven.Client.Document;

namespace HashBus.View.MentionLeaderboard
{

    class App
    {
        public static async Task RunAsync(string ravenDBUrl, string ravenDBDatabase, string hashtag, int refreshInterval)
        {
            using (var store = new DocumentStore())
            {
                store.Url = ravenDBUrl;
                store.DefaultDatabase = ravenDBDatabase;
                store.EnlistInDistributedTransactions = false;
                store.Initialize();
                await View.StartAsync(
                    hashtag,
                    refreshInterval,
                    new RavenDBListRepository<Mention>(store));
            }
        }
    }
}
