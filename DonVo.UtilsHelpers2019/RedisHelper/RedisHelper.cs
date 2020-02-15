using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using Timer = System.Timers.Timer;

namespace RedisHelper
{
    public class RedisHelper
    {
        private readonly ConnectionMultiplexer _conn;
        public int DbNumber { get; set; } = -1;

        public RedisHelper(string connectionString) => _conn = ConnectionMultiplexer.Connect(connectionString);

        private IDatabase Db => _conn.GetDatabase(DbNumber);

        #region String

        public async Task<bool> StringSetAsync<T>(string key, T value) =>
            await Db.StringSetAsync(key, value.ToRedisValue());

        public async Task<T> StringGetAsync<T>(string key) where T : class =>
            (await Db.StringGetAsync(key)).ToObject<T>();

        public async Task<double> StringIncrementAsync(string key, int value = 1) =>
            await Db.StringIncrementAsync(key, value);

        public async Task<double> StringDecrementAsync(string key, int value = 1) =>
            await Db.StringDecrementAsync(key, value);

        #endregion

        #region List

        public async Task<long> EnqueueAsync<T>(string key, T value) =>
            await Db.ListRightPushAsync(key, value.ToRedisValue());

        public async Task<T> DequeueAsync<T>(string key) where T : class =>
            (await Db.ListLeftPopAsync(key)).ToObject<T>();

        /// <summary>
        /// Read data from the queue without leaving the team
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start">starting point</param>
        /// <param name="stop">End position</param>
        /// <typeparam name="T">Object type</typeparam>
        /// <returns>Not specify start、end Get all the data</returns>
        public async Task<IEnumerable<T>> PeekRangeAsync<T>(string key, long start = 0, long stop = -1)
            where T : class =>
            (await Db.ListRangeAsync(key, start, stop)).ToObjects<T>();

        #endregion

        #region Set

        public async Task<bool> SetAddAsync<T>(string key, T value) =>
            await Db.SetAddAsync(key, value.ToRedisValue());

        public async Task<long> SetRemoveAsync<T>(string key, IEnumerable<T> values) =>
            await Db.SetRemoveAsync(key, values.ToRedisValues());

        public async Task<IEnumerable<T>> SetMembersAsync<T>(string key) where T : class =>
            (await Db.SetMembersAsync(key)).ToObjects<T>();

        public async Task<bool> SetContainsAsync<T>(string key, T value) =>
            await Db.SetContainsAsync(key, value.ToRedisValue());

        #endregion

        #region Sortedset

        public async Task<bool> SortedSetAddAsync(string key, string member, double score) =>
            await Db.SortedSetAddAsync(key, member, score);

        public async Task<long> SortedSetRemoveAsync(string key, IEnumerable<string> members) =>
            await Db.SortedSetRemoveAsync(key, members.ToRedisValues());

        public async Task<double> SortedSetIncrementAsync(string key, string member, double value) =>
            await Db.SortedSetIncrementAsync(key, member, value);

        public async Task<double> SortedSetDecrementAsync(string key, string member, double value) =>
            await Db.SortedSetDecrementAsync(key, member, value);

        /// <summary>
        /// Return to topN in order
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, double>> SortedSetRangeByRankWithScoresAsync(string key, long start = 0,
            long stop = -1,
            Order order = Order.Ascending) =>
            (await Db.SortedSetRangeByRankWithScoresAsync(key, start, stop, order)).ToDictionary();

        public async Task<Dictionary<string, double>> SortedSetRangeByScoreWithScoresAsync(string key,
            double start = double.NegativeInfinity, double stop = double.PositiveInfinity,
            Exclude exclude = Exclude.None, Order order = Order.Ascending, long skip = 0, long take = -1) =>
            (await Db.SortedSetRangeByScoreWithScoresAsync(key, start, stop, exclude, order, skip, take))
            .ToDictionary();

        #endregion

        #region Hash

        public async Task<Dictionary<string, string>> HashGetAsync(string key) =>
            (await Db.HashGetAllAsync(key)).ToDictionary();

        public async Task<Dictionary<string, string>> HashGetFieldsAsync(string key, IEnumerable<string> fields) =>
            (await Db.HashGetAsync(key, fields.ToRedisValues())).ToDictionary(fields);

        public async Task HashSetAsync(string key, Dictionary<string, string> entries) =>
            await Db.HashSetAsync(key, entries.ToHashEntries());

        public async Task HashSetFieldsAsync(string key, Dictionary<string, string> fields)
        {
            var hs = await HashGetAsync(key);
            foreach (var field in fields)
                hs[field.Key] = field.Value;

            await HashSetAsync(key, hs);
        }

        public async Task<bool> HashDeleteAsync(string key) =>
            await KeyDeleteAsync(new string[] { key }) > 0;

        public async Task<bool> HashDeleteFieldsAsync(string key, IEnumerable<string> fields)
        {
            foreach (var field in fields)
                if (!await Db.HashDeleteAsync(key, field))
                    return false;

            return true;
        }

        #endregion

        #region Key

        public IEnumerable<string> GetAllKeys() =>
            _conn.GetEndPoints().Select(endPoint => _conn.GetServer(endPoint))
                .SelectMany(server => server.Keys().ToStrings());

        public IEnumerable<string> GetAllKeys(EndPoint endPoint) =>
            _conn.GetServer(endPoint).Keys().ToStrings();

        public async Task<bool> KeyExistsAsync(string key) =>
            await Db.KeyExistsAsync(key);

        /// <summary>
        /// Delete given key
        /// </summary>
        /// <param name="keys">Key collection to be deleted</param>
        /// <returns>Delete the number of keys</returns>
        public async Task<long> KeyDeleteAsync(IEnumerable<string> keys) =>
            await Db.KeyDeleteAsync(keys.Select(k => (RedisKey)k).ToArray());

        /// <summary>
        /// Set the specified key expiration time
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public async Task<bool> KeyExpireAsync(string key, TimeSpan? expiry) => await Db.KeyExpireAsync(key, expiry);

        public async Task<bool> KeyExpireAsync(string key, DateTime? expiry) => await Db.KeyExpireAsync(key, expiry);

        #endregion

        #region Advanced

        public async Task<long> PublishAsync(string channel, string msg) =>
            await _conn.GetSubscriber().PublishAsync(channel, msg);

        public async Task SubscribeAsync(string channel, Action<string, string> handler)
        {
            await _conn.GetSubscriber().SubscribeAsync(channel, (chn, msg) => handler(chn, msg));
        }

        /// <summary>
        /// Perform Redis operations in batches
        /// </summary>
        /// <param name="operations"></param>
        public Task ExecuteBatchAsync(params Action[] operations) =>
            Task.Run(() =>
            {
                var batch = Db.CreateBatch();

                foreach (var operation in operations)
                    operation();

                batch.Execute();
            });


        /// <summary>
        /// Get the distributed lock and execute it (non-blocking. Locking failure returns directly (false, null))
        /// </summary>
        /// <param name="key">The key to lock</param>
        /// <param name="value">The value of the lock, the value is assigned when the lock is applied, and the client with the same value must be unlocked when unlocking.</param>
        /// <param name="del">Business method executed when the lock is successful</param>
        /// <param name="expiry">Hold lock timeout. Automatic release of lock after timeout</param>
        /// <param name="args">Business method parameters</param>
        /// <returns>(success,return value of the del)</returns>
        public async Task<(bool, object)> LockExecuteAsync(string key, string value, Delegate del,
            TimeSpan expiry, params object[] args)
        {
            if (!await Db.LockTakeAsync(key, value, expiry))
                return (false, null);

            try
            {
                return (true, del.DynamicInvoke(args));
            }
            finally
            {
                Db.LockRelease(key, value);
            }
        }

        /// <summary>
        /// Get distributed locks and execute (blocking. Until successful locking or timeout)
        /// </summary>
        /// <param name="key">The key to lock</param>
        /// <param name="value">The value of the lock, the value is assigned when the lock is applied, and the client with the same value must be unlocked when unlocking.</param>
        /// <param name="del">Business method executed when the lock is successful</param>
        /// <param name="result">Del return value</param>
        /// <param name="expiry">Hold lock timeout. Automatic release of lock after timeout</param>
        /// <param name="timeout">Lock timeout (ms).0 means never timeout</param>
        /// <param name="args">Business method parameters</param>
        /// <returns>success</returns>
        public bool LockExecute(string key, string value, Delegate del, out object result, TimeSpan expiry,
            int timeout = 0, params object[] args)
        {
            result = null;
            if (!GetLock(key, value, expiry, timeout))
                return false;

            try
            {
                result = del.DynamicInvoke(args);
                return true;
            }
            finally
            {
                Db.LockRelease(key, value);
            }
        }

        public bool LockExecute(string key, string value, Action action, TimeSpan expiry, int timeout = 0)
        {
            return LockExecute(key, value, action, out var _, expiry, timeout);
        }

        public bool LockExecute<T>(string key, string value, Action<T> action, T arg, TimeSpan expiry, int timeout = 0)
        {
            return LockExecute(key, value, action, out var _, expiry, timeout, arg);
        }

        public bool LockExecute<T>(string key, string value, Func<T> func, out T result, TimeSpan expiry,
            int timeout = 0)
        {
            result = default;
            if (!GetLock(key, value, expiry, timeout))
                return false;
            try
            {
                result = func();
                return true;
            }
            finally
            {
                Db.LockRelease(key, value);
            }
        }

        public bool LockExecute<T, TResult>(string key, string value, Func<T, TResult> func, T arg, out TResult result,
            TimeSpan expiry, int timeout = 0)
        {
            result = default;
            if (!GetLock(key, value, expiry, timeout))
                return false;
            try
            {
                result = func(arg);
                return true;
            }
            finally
            {
                Db.LockRelease(key, value);
            }
        }

        private bool GetLock(string key, string value, TimeSpan expiry, int timeout)
        {
            using (var waitHandle = new AutoResetEvent(false))
            {
                var timer = new Timer(1000);
                timer.Start();
                timer.Elapsed += (s, e) =>
                {
                    if (!Db.LockTake(key, value, expiry))
                        return;
                    try
                    {
                        waitHandle.Set();
                        timer.Stop();
                    }
                    catch
                    {
                    }
                };


                if (timeout > 0)
                    waitHandle.WaitOne(timeout);
                else
                    waitHandle.WaitOne();

                timer.Stop();
                timer.Close();
                timer.Dispose();

                return Db.LockQuery(key) == value;
            }
        }

        #endregion
    }

    public static class StackExchangeRedisExtension
    {
        public static IEnumerable<string> ToStrings(this IEnumerable<RedisKey> keys) => keys.Select(k => (string)k);

        public static RedisValue ToRedisValue<T>(this T value) =>
            value is ValueType || value is string
                ? value.ToString()
                : JsonConvert.SerializeObject(value);


        public static RedisValue[] ToRedisValues<T>(this IEnumerable<T> values) =>
            values.Select(v => v.ToRedisValue()).ToArray();

        public static T ToObject<T>(this RedisValue value) where T : class => typeof(T) == typeof(string)
            ? value.ToString() as T
            : JsonConvert.DeserializeObject<T>(value.ToString());

        public static IEnumerable<T> ToObjects<T>(this IEnumerable<RedisValue> values) where T : class =>
            values.Select(v => v.ToObject<T>());

        public static HashEntry[] ToHashEntries(this Dictionary<string, string> entries)
        {
            var es = new HashEntry[entries.Count];
            for (var i = 0; i < entries.Count; i++)
            {
                var name = entries.Keys.ElementAt(i);
                var value = entries[name];
                es[i] = new HashEntry(name, value);
            }

            return es;
        }

        public static Dictionary<string, string> ToDictionary(this IEnumerable<HashEntry> entries)
        {
            var dict = new Dictionary<string, string>();
            foreach (var entry in entries)
                dict[entry.Name] = entry.Value;

            return dict;
        }

        public static Dictionary<string, string> ToDictionary(this RedisValue[] hashValues, IEnumerable<string> fields)
        {
            var dict = new Dictionary<string, string>();
            for (var i = 0; i < fields.Count(); i++)
                dict[fields.ElementAt(i)] = hashValues[i];

            return dict;
        }

        public static Dictionary<string, double> ToDictionary(this IEnumerable<SortedSetEntry> entries)
        {
            var dict = new Dictionary<string, double>();
            foreach (var entry in entries)
                dict[entry.Element] = entry.Score;

            return dict;
        }
    }
}
