using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using System;
using System.Text;
using System.Threading.Tasks;

namespace RedisCacheExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RunSampleAsync().Wait();
        }

        /// <summary>
        /// Install Redis Server using Redis-x64-3.2.100.msi
        /// This sample assumes that a redis server is running on the local machine. You can set this up by doing the following:
        /// Install this chocolatey package: http://chocolatey.org/packages/redis-64/
        /// run "redis-server" from command prompt.
        /// </summary>
        /// <param name="args"></param>
        public static async Task RunSampleAsync()
        {
            var key = "myKey";

            var message = "Meghan Markle seems to have a few non-negotiable rules when she travels. Number one, she's always dressed like a walking outfit inspo Pinterest board. Number two, she must make time for some self care. The Duchess of Sussex took a last - minute flight to New York City on Friday, September 6 to watch her close friend, Serena Williams, play in the U.S.Open finals. Before her Saturday afternoon appointment at the highly anticipated match, People reports that Meghan Markle and an unnamed friend dropped by Modo Yoga, an NYC studio that includes a specially heated room, for class on Friday evening. Despite her high profile, Markle reportedly took her hot yoga class alongside 60 other people.But it seems no one asked Markle what she thinks of Brexit or the upcoming season of The Crown during savasana. A source told People that there were lots of sweet, knowing smiles between patrons—and that was it.";

            var value = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("Connecting to cache");
            var cache = new RedisCache(new RedisCacheOptions
            {
                Configuration = "localhost",
                InstanceName = "SampleInstance"
            });
            Console.WriteLine("Connected");

            Console.WriteLine($"Setting value '{message}' in cache");
            await cache.SetAsync(key, value, new DistributedCacheEntryOptions());
            Console.WriteLine("Set");

            Console.WriteLine("Getting value from cache");
            value = await cache.GetAsync(key);
            if (value != null)
            {
                Console.WriteLine("Retrieved: " + Encoding.UTF8.GetString(value));
            }
            else
            {
                Console.WriteLine("Not Found");
            }

            Console.WriteLine("Refreshing value in cache");
            await cache.RefreshAsync(key);
            Console.WriteLine("Refreshed");

            Console.WriteLine("Removing value from cache");
            await cache.RemoveAsync(key);
            Console.WriteLine("Removed");

            Console.WriteLine("Getting value from cache again");
            value = await cache.GetAsync(key);
            if (value != null)
            {
                Console.WriteLine("Retrieved: " + Encoding.UTF8.GetString(value));
            }
            else
            {
                Console.WriteLine("Not Found");
            }

            Console.ReadLine();
        }
    }
}
