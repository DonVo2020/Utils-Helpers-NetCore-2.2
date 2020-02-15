using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.SqlServer;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SqlServerCacheSample
{
    /// <summary>
    /// This sample requires setting up a Microsoft SQL Server based cache database.
    /// 1. Install the .NET Core sql-cache tool globally by installing the dotnet-sql-cache package.
    ///     using cmd:  dotnet tool install --global dotnet-sql-cache --version 2.2.0
    /// 2. Create a new database in the SQL Server or use an existing one.
    /// 3. dotnet sql-cache create "Data Source=LAPTOP-ILQS92OM\SQLEXPRESS;Initial Catalog=DapperDB;Integrated Security=True;" dbo TestCache
    /// 4. Run this sample by doing "dotnet run"
    /// </summary>
    public class Program
    {
        private const string Key = "MyKey";
        private static readonly Random Random = new Random();
        private static DistributedCacheEntryOptions _cacheEntryOptions;

        public static void Main()
        {
            RunSqlServerCacheAsync().Wait();

            Console.WriteLine("-------------- End of RunSqlServerCacheAsync --------------\n\n");
            Console.WriteLine("Start RunSqlServerCacheConcurrency. Press Enter key to Exit.\n\n");
            RunSqlServerCacheConcurrency();
        }

        public static async Task RunSqlServerCacheAsync()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var configuration = configurationBuilder
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();

            var key = Guid.NewGuid().ToString();
            var message = "Hello, World!";
            var value = Encoding.UTF8.GetBytes(message);

            Console.WriteLine("Connecting to cache");
            var cache = new SqlServerCache(new SqlServerCacheOptions()
            {
                ConnectionString = configuration["ConnectionString"],
                SchemaName = configuration["SchemaName"],
                TableName = configuration["TableName"]
            });

            Console.WriteLine("Connected");

            Console.WriteLine("Cache item key: {0}", key);
            Console.WriteLine($"Setting value '{message}' in cache");
            await cache.SetAsync(
                key,
                value,
                new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(10)));
            Console.WriteLine("Set");

            Console.WriteLine("Getting value from cache");
            value = await cache.GetAsync(key);
            if (value != null)
            {
                Console.WriteLine("Retrieved: " + Encoding.UTF8.GetString(value, 0, value.Length));
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
                Console.WriteLine("Retrieved: " + Encoding.UTF8.GetString(value, 0, value.Length));
            }
            else
            {
                Console.WriteLine("Not Found");
            }

            Console.WriteLine("\nPress Enter key to continue ...");
            Console.ReadLine();
        }


        #region <=========== SqlServerCacheConcurrency ===========>
        public static void RunSqlServerCacheConcurrency()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddJsonFile("config.json")
                .AddEnvironmentVariables();
            var configuration = configurationBuilder.Build();

            _cacheEntryOptions = new DistributedCacheEntryOptions();
            _cacheEntryOptions.SetSlidingExpiration(TimeSpan.FromSeconds(10));

            var cache = new SqlServerCache(new SqlServerCacheOptions()
            {
                ConnectionString = configuration["ConnectionString"],
                SchemaName = configuration["SchemaName"],
                TableName = configuration["TableName"]
            });

            SetKey(cache, "0");

            PeriodicallyReadKey(cache, TimeSpan.FromSeconds(1));

            PeriodciallyRemoveKey(cache, TimeSpan.FromSeconds(11));

            PeriodciallySetKey(cache, TimeSpan.FromSeconds(13));

            Console.ReadLine();
            Console.WriteLine("Shutting down");
        }

        private static void SetKey(IDistributedCache cache, string value)
        {
            Console.WriteLine("Setting: " + value);
            cache.Set(Key, Encoding.UTF8.GetBytes(value), _cacheEntryOptions);
        }
        private static void PeriodciallySetKey(IDistributedCache cache, TimeSpan interval)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(interval);

                    SetKey(cache, "A");
                }
            });
        }
        private static void PeriodicallyReadKey(IDistributedCache cache, TimeSpan interval)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(interval);

                    if (Random.Next(3) == 0) // 1/3 chance
                    {
                        // Allow values to expire due to sliding refresh.
                        Console.WriteLine("Read skipped, random choice.");
                    }
                    else
                    {
                        Console.Write("Reading...");
                        object result = cache.Get(Key);
                        if (result != null)
                        {
                            cache.Set(Key, Encoding.UTF8.GetBytes("B"), _cacheEntryOptions);
                        }
                        Console.WriteLine("Read: " + (result ?? "(null)"));
                    }
                }
            });
        }
        private static void PeriodciallyRemoveKey(IDistributedCache cache, TimeSpan interval)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(interval);

                    Console.WriteLine("Removing...");
                    cache.Remove(Key);
                }
            });
        }
        #endregion
    }
}
