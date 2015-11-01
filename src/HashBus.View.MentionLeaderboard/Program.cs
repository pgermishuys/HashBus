using System.Configuration;

namespace HashBus.View.MentionLeaderboard
{
    class Program
    {
        static void Main()
        {
            var ravenDBUrl = ConfigurationManager.AppSettings["RavenDBUrl"];
            var ravenDBDatabase = ConfigurationManager.AppSettings["RavenDBDatabase"];
            var hashTag = ConfigurationManager.AppSettings["hashTag"];
            var refreshInterval = int.Parse(ConfigurationManager.AppSettings["refreshInterval"]);
            App.RunAsync(ravenDBUrl, ravenDBDatabase, hashTag, refreshInterval).GetAwaiter().GetResult();
        }
    }
}
