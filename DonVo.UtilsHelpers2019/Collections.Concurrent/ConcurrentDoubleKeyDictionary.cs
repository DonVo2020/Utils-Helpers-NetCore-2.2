using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Collections.Concurrent
{
    public class ConcurrentDoubleKeyDictionary<TKey1, TKey2, TValue> : IEquatable<ConcurrentDoubleKeyDictionary<TKey1, TKey2, TValue>>, IEnumerable<KeyValuePair<(TKey1, TKey2), TValue>>
    {
        public int Count => checked(objects.Values.Sum((v) => v.Count));
        public long LongCount => checked(objects.Values.Sum((v) => (long)v.Count));
        public bool IsEmpty => objects.Values.All((v) => v.IsEmpty);
        public IEnumerable<(TKey1, TKey2)> Keys => objects.SelectMany((kvp1) => kvp1.Value.Select((kvp2) => (kvp1.Key, kvp2.Key)));
        public IEnumerable<TValue> Values => objects.Values.SelectMany((v1) => v1.Select((kvp2) => kvp2.Value));

        public ConcurrentDictionary<TKey2, TValue> this[TKey1 key1]
        {
            get { return objects[key1]; }
        }
        public TValue this[TKey1 key1, TKey2 key2]
        {
            get { return objects[key1][key2]; }
            set { objects[key1][key2] = value; }
        }

        private readonly Func<TKey1, TKey2, bool> keyRestriction;
        private readonly ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>> objects = new ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>>();

        public ConcurrentDoubleKeyDictionary(Func<TKey1, TKey2, bool> keyRestriction = null)
        {
            this.keyRestriction = keyRestriction;
        }

        public TValue AddOrUpdate(TKey1 key1, TKey2 key2, TValue addValue, Func<TKey1, TKey2, TValue, TValue> updateFactory)
        {
            ValidateKey(key1, key2);

            return GetOrCreateFirstKey(key1).AddOrUpdate(key2, addValue, (k2, oldValue) => updateFactory(key1, k2, oldValue));
        }
        public TValue AddOrUpdate(TKey1 key1, TKey2 key2, Func<TKey1, TKey2, TValue> addFactory, Func<TKey1, TKey2, TValue, TValue> updateFactory)
        {
            ValidateKey(key1, key2);

            return GetOrCreateFirstKey(key1).AddOrUpdate(key2, (k2) => addFactory(key1, k2), (k2, oldValue) => updateFactory(key1, k2, oldValue));
        }
        public void Clear()
        {
            objects.Clear();
        }
        public bool ContainsKey(TKey1 key)
        {
            return objects.TryGetValue(key, out ConcurrentDictionary<TKey2, TValue> innerDict) && innerDict.Count > 0;
        }
        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            return objects.TryGetValue(key1, out ConcurrentDictionary<TKey2, TValue> innerDict) && innerDict.ContainsKey(key2);
        }
        public ICollection<TKey2> GetAllSubKeys(TKey1 key)
        {
            return objects.TryGetValue(key, out ConcurrentDictionary<TKey2, TValue> innerDict) ?
                innerDict.Keys : Array.Empty<TKey2>();
        }
        public ICollection<TValue> GetAllSubValues(TKey1 key)
        {
            return objects.TryGetValue(key, out ConcurrentDictionary<TKey2, TValue> innerDict) ?
                innerDict.Values : Array.Empty<TValue>();
        }
        public IEnumerable<KeyValuePair<TKey2, TValue>> GetAllSubData(TKey1 key)
        {
            return objects.TryGetValue(key, out ConcurrentDictionary<TKey2, TValue> innerDict) ?
                innerDict : Enumerable.Empty<KeyValuePair<TKey2, TValue>>();
        }
        public TValue GetOrAdd(TKey1 key1, TKey2 key2, TValue value)
        {
            ValidateKey(key1, key2);

            return GetOrCreateFirstKey(key1).GetOrAdd(key2, value);
        }
        public TValue GetOrAdd(TKey1 key1, TKey2 key2, Func<TKey1, TKey2, TValue> valueFactory)
        {
            ValidateKey(key1, key2);

            return GetOrCreateFirstKey(key1).GetOrAdd(key2, (k2) => valueFactory(key1, k2));
        }
        public bool TryAdd(TKey1 key1, TKey2 key2, TValue value)
        {
            ValidateKey(key1, key2);

            return GetOrCreateFirstKey(key1).TryAdd(key2, value);
        }
        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
        {
            value = default;
            return objects.TryGetValue(key1, out ConcurrentDictionary<TKey2, TValue> innerDict) && innerDict.TryGetValue(key2, out value);
        }
        public bool TryGetValue(TKey1 key1, out ConcurrentDictionary<TKey2, TValue> value)
        {
            return objects.TryGetValue(key1, out value);
        }
        public bool TryRemove(TKey1 key1, TKey2 key2, out TValue value)
        {
            value = default;
            return objects.TryGetValue(key1, out ConcurrentDictionary<TKey2, TValue> innerDict) && innerDict.TryRemove(key2, out value);
        }
        public bool TryRemove(TKey1 key1, out ConcurrentDictionary<TKey2, TValue> value)
        {
            if (objects.TryRemove(key1, out value) && value.Count > 0)
            {
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public bool TryUpdate(TKey1 key1, TKey2 key2, TValue newValue, TValue comparisonValue)
        {
            return objects.TryGetValue(key1, out ConcurrentDictionary<TKey2, TValue> innerDict) && innerDict.TryUpdate(key2, newValue, comparisonValue);
        }

        public bool Equals(ConcurrentDoubleKeyDictionary<TKey1, TKey2, TValue> other)
        {
            if (other is null)
            {
                return false;
            }

            return this.OrderBy(kvp => kvp.Key).SequenceEqual(other.OrderBy(kvp => kvp.Key));
        }

        public IEnumerator<KeyValuePair<(TKey1, TKey2), TValue>> GetEnumerator()
        {
            return objects.SelectMany((kvp1) => kvp1.Value.Select((kvp2) => new KeyValuePair<(TKey1, TKey2), TValue>((kvp1.Key, kvp2.Key), kvp2.Value))).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void ValidateKey(TKey1 key1, TKey2 key2)
        {
            if (keyRestriction?.Invoke(key1, key2) ?? true)
            {
                return;
            }

            throw new ArgumentException("Key1 is invalid");
        }
        private ConcurrentDictionary<TKey2, TValue> GetOrCreateFirstKey(TKey1 key1)
        {
            return objects.GetOrAdd(key1, (_) => new ConcurrentDictionary<TKey2, TValue>());
        }
    }
}
