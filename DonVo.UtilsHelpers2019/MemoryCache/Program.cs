using System.Collections.Generic;

using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace MemoryCacheSample
{
    class Program
    {
        private const string Key = "MyKey";
        private static readonly Random Random = new Random();
        private static MemoryCacheEntryOptions _cacheEntryOptions;

        static void Main(string[] args)
        {
            Sample01();
            Console.WriteLine("================== Sample 02 ==================");
            Sample02();

            Console.WriteLine("\nStart RunSqlServerCacheConcurrency. Press Enter key to Exit.\n\n");
            Console.WriteLine("================== MemoryCacheConcurencySample ==================");
            MemoryCacheConcurencySample();
        }

        private static void Sample01()
        {
            StockItems PS = new StockItems();
            _ = (List<string>)PS.GetAvailableStocks();
            _ = (List<string>)PS.GetAvailableStocks();
        }

        private static void Sample02()
        {
            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());
            object result;
            string key = "Key";
            object newObject = new object();
            object state = new object();

            // Basic CRUD operations:

            // Create / Overwrite
            result = cache.Set(key, newObject);
            result = cache.Set(key, new object());

            // Retrieve, null if not found
            result = cache.Get(key);

            // Retrieve
            bool found = cache.TryGetValue(key, out result);

            // Store and Get using weak references
            result = cache.SetWeak<object>(key, newObject);
            result = cache.GetWeak<object>(key);

            // Delete
            cache.Remove(key);

            // Cache entry configuration:

            // Stays in the cache as long as possible
            result = cache.Set(
                key,
                new object(),
                new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove));

            // Automatically remove if not accessed in the given time
            result = cache.Set(
                key,
                new object(),
                new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)));

            // Automatically remove at a certain time
            result = cache.Set(
                key,
                new object(),
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddDays(2)));

            // Automatically remove at a certain time, which is relative to UTC now
            result = cache.Set(
                key,
                new object(),
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(relative: TimeSpan.FromMinutes(10)));

            // Automatically remove if not accessed in the given time
            // Automatically remove at a certain time (if it lives that long)
            result = cache.Set(
                key,
                new object(),
                new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddDays(2)));

            // Callback when evicted
            var options = new MemoryCacheEntryOptions()
                .RegisterPostEvictionCallback(
                (echoKey, value, reason, substate) =>
                {
                    Console.WriteLine(echoKey + ": '" + value + "' was evicted due to " + reason);
                });
            result = cache.Set(key, new object(), options);

            // Remove on token expiration
            var cts = new CancellationTokenSource();
            options = new MemoryCacheEntryOptions()
                .AddExpirationToken(new CancellationChangeToken(cts.Token))
                .RegisterPostEvictionCallback(
                (echoKey, value, reason, substate) =>
                {
                    Console.WriteLine(echoKey + ": '" + value + "' was evicted due to " + reason);
                });
            result = cache.Set(key, new object(), options);

            // Fire the token to see the registered callback being invoked
            cts.Cancel();

            // Expire an entry if the dependent entry expires
            using (var entry = cache.CreateEntry("key1"))
            {
                // expire this entry if the entry with key "key2" expires.
                entry.Value = "value1";
                cts = new CancellationTokenSource();
                entry.RegisterPostEvictionCallback(
                    (echoKey, value, reason, substate) =>
                    {
                        Console.WriteLine(echoKey + ": '" + value + "' was evicted due to " + reason);
                    });

                cache.Set("key2", "value2", new CancellationChangeToken(cts.Token));
            }

            // Fire the token to see the registered callback being invoked
            cts.Cancel();
        }

        private static void MemoryCacheConcurencySample()
        {
            _cacheEntryOptions = GetCacheEntryOptions();

            IMemoryCache cache = new MemoryCache(new MemoryCacheOptions());

            SetKey(cache, "0");

            PeriodicallyReadKey(cache, TimeSpan.FromSeconds(1));

            PeriodicallyRemoveKey(cache, TimeSpan.FromSeconds(11));

            PeriodicallySetKey(cache, TimeSpan.FromSeconds(13));

            Console.ReadLine();
            Console.WriteLine("Shutting down");
        }

        private static void SetKey(IMemoryCache cache, string value)
        {
            Console.WriteLine("Setting: " + value);
            cache.Set(Key, value, _cacheEntryOptions);
        }
        private static MemoryCacheEntryOptions GetCacheEntryOptions()
        {
            return new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(7))
                .SetSlidingExpiration(TimeSpan.FromSeconds(3))
                .RegisterPostEvictionCallback(AfterEvicted, state: null);
        }
        private static void AfterEvicted(object key, object value, EvictionReason reason, object state)
        {
            Console.WriteLine("Evicted. Value: " + value + ", Reason: " + reason);
        }
        private static void PeriodicallySetKey(IMemoryCache cache, TimeSpan interval)
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
        private static void PeriodicallyReadKey(IMemoryCache cache, TimeSpan interval)
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
                        if (!cache.TryGetValue(Key, out object result))
                        {
                            result = cache.Set(Key, "B", _cacheEntryOptions);
                        }
                        Console.WriteLine("Read: " + (result ?? "(null)"));
                    }
                }
            });
        }
        private static void PeriodicallyRemoveKey(IMemoryCache cache, TimeSpan interval)
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
    }
}
