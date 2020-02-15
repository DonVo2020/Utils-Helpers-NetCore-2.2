using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace MemoryCacheSample
{
    public class StockItems
    {
        private const string CacheKey = "availableStocks";

        public IEnumerable GetAvailableStocks()
        {
            ObjectCache cache = MemoryCache.Default;

            if (cache.Contains(CacheKey))
            {
                Console.WriteLine("\n\nRetrieve Data using Cache Key: " + CacheKey);
                Console.Write("Return result: ");
                var list = (List<string>)cache.Get(CacheKey);
                Console.WriteLine(string.Join("\t", list));
                Console.WriteLine("------------------------------------------------\n");
                //Console.WriteLine("Return Result: " + (IEnumerable)cache.Get(CacheKey));
                return (IEnumerable)cache.Get(CacheKey);
            }
            else
            {
                IEnumerable availableStocks = this.GetDefaultStocks();

                // Store data in the cache    
                CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
                cacheItemPolicy.AbsoluteExpiration = DateTime.Now.AddHours(1.0);
                cache.Add(CacheKey, availableStocks, cacheItemPolicy);

                Console.Write("Store Data in Cache: ");
                Console.WriteLine(string.Join("\t", (List<string>)availableStocks));
                Console.WriteLine("------------------------------------------------");
                //Console.WriteLine("Store Data in Cache: " + availableStocks);

                return availableStocks;
            }
        }
        public IEnumerable GetDefaultStocks()
        {
            return new List<string>() { "Pen", "Pencil", "Eraser" };
        }
    }
}
