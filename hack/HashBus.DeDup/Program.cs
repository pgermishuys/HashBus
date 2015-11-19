namespace HashBus.DeDup
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HashBus.ReadModel;
    using HashBus.ReadModel.MongoDB;
    using MongoDB.Driver;

    class Program
    {
        static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();
            Console.WriteLine("Tantalise any key to exit.");
            Console.ReadKey();
        }

        private static async Task RunAsync()
        {
            var track = "#BuildStuffLT";
            var database = new MongoClient("mongodb://localhost:27017").GetDatabase("hashbus_readmodel");

            await DeDupMentions(track, database);
            await DeDupHashtags(track, database);
        }

        private static async Task DeDupMentions(string track, IMongoDatabase database)
        {
            var mentions = new MongoDBListRepository<Mention>(database, "most_mentioned__mentions");

            var trackMentions = (await mentions.GetAsync(track)).ToList();
            Console.WriteLine($"Found {trackMentions.Count} {track} mentions.");

            var deDupedTrackMentions = trackMentions
                .GroupBy(mention => new { mention.TweetId, mention.UserMentionId })
                .Select(group => group.First())
                .ToList();

            Console.WriteLine($"Deduped to {deDupedTrackMentions.Count} {track} mentions.");
            await mentions.SaveAsync(track, deDupedTrackMentions);
        }

        private static async Task DeDupHashtags(string track, IMongoDatabase database)
        {
            var hashtags = new MongoDBListRepository<Hashtag>(database, "most_hashtagged__hashtags");
            var trackHashtags = (await hashtags.GetAsync(track)).ToList();
            Console.WriteLine($"Found {trackHashtags.Count} {track} hashtag usages.");

            var deDupedHashtags = trackHashtags
                .GroupBy(mention => new { mention.TweetId, Text = mention.Text.ToUpperInvariant() })
                .Select(group => group.First())
                .ToList();

            Console.WriteLine($"Deduped to {deDupedHashtags.Count} {track} hashtags.");
            await hashtags.SaveAsync(track, deDupedHashtags);
        }
    }
}
