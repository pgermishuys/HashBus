using System.Configuration;

namespace HashBus.Projection.UserLeaderboard
{
    class Program
    {
        static void Main()
        {
            var ravenDBUrl = ConfigurationManager.AppSettings["RavenDBUrl"];
            var ravenDBDatabase = ConfigurationManager.AppSettings["RavenDBDatabase"];

            App.RunAsync(ravenDBUrl, ravenDBDatabase).GetAwaiter().GetResult();
        }
    }
}
