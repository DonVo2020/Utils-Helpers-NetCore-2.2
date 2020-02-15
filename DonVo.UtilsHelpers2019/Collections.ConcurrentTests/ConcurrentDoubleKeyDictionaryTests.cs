using System;
using System.Linq;
using Collections.Concurrent;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    internal class ConcurrentDoubleKeyDictionaryTests
    {
        [Test]
        //[Parallelizable(ParallelScope.All)]
        public void TestHappyFlow()
        {
            int result;
            ConcurrentDoubleKeyDictionary<int, int, int> dictionary = new ConcurrentDoubleKeyDictionary<int, int, int>();

            Assert.IsEmpty(dictionary.Keys);
            Assert.IsEmpty(dictionary.Values);

            Assert.IsFalse(dictionary.TryGetValue(1, 1, out _));
            Assert.IsTrue(dictionary.TryAdd(1, 1, 1));
            Assert.AreEqual(1, dictionary.Keys.Count());
            Assert.AreEqual(1, dictionary.Values.Count());
            Assert.IsTrue(dictionary.TryGetValue(1, 1, out result));
            Assert.AreEqual(1, result);
            Assert.IsFalse(dictionary.TryAdd(1, 1, 1));
            Assert.AreEqual(1, dictionary.Keys.Count());
            Assert.AreEqual(1, dictionary.Values.Count());
            Assert.IsTrue(dictionary.TryGetValue(1, 1, out result));
            Assert.AreEqual(1, result);

            Assert.IsTrue(dictionary.TryUpdate(1, 1, 2, 1));
            Assert.AreEqual(1, dictionary.Keys.Count());
            Assert.AreEqual(1, dictionary.Values.Count());
            Assert.IsTrue(dictionary.TryGetValue(1, 1, out result));
            Assert.AreEqual(2, result);
            Assert.IsFalse(dictionary.TryUpdate(1, 1, 2, 1));
            Assert.AreEqual(1, dictionary.Keys.Count());
            Assert.AreEqual(1, dictionary.Values.Count());
            Assert.IsTrue(dictionary.TryGetValue(1, 1, out result));
            Assert.AreEqual(2, result);

            Assert.IsFalse(dictionary.TryRemove(1, 2, out _));
            Assert.AreEqual(1, dictionary.Keys.Count());
            Assert.AreEqual(1, dictionary.Values.Count());
            Assert.IsTrue(dictionary.TryGetValue(1, 1, out result));
            Assert.AreEqual(2, result);
            Assert.IsTrue(dictionary.TryRemove(1, 1, out result));
            Assert.AreEqual(2, result);
            Assert.IsEmpty(dictionary.Keys);
            Assert.IsEmpty(dictionary.Values);
            Assert.IsFalse(dictionary.TryGetValue(1, 1, out _));
            Assert.IsFalse(dictionary.TryRemove(1, 2, out _));
            Assert.IsEmpty(dictionary.Keys);
            Assert.IsEmpty(dictionary.Values);
            Assert.IsFalse(dictionary.TryGetValue(1, 1, out _));
        }
        [Test]
        public void TestKey1Filter()
        {
            ConcurrentDoubleKeyDictionary<int, int, int> dictionary = new ConcurrentDoubleKeyDictionary<int, int, int>((k1, k2) => k1 > 0);
            Assert.IsTrue(dictionary.TryAdd(1, 1, 1));
            Assert.IsTrue(dictionary.TryAdd(1, -1, 1));
            Assert.Throws<ArgumentException>(() => dictionary.TryAdd(-1, 1, 1));
            Assert.Throws<ArgumentException>(() => dictionary.TryAdd(-1, -1, 1));
            Assert.IsTrue(dictionary.ContainsKey(1, 1));
            Assert.IsTrue(dictionary.ContainsKey(1, -1));
            Assert.IsFalse(dictionary.ContainsKey(-1, 1));
            Assert.IsFalse(dictionary.ContainsKey(-1, -1));

            dictionary = new ConcurrentDoubleKeyDictionary<int, int, int>((k1, k2) => k1 > 0);
            Assert.IsTrue(dictionary.AddOrUpdate(1, 1, 1, (_, _1, _2) => 2) == 1);
            Assert.IsTrue(dictionary.AddOrUpdate(1, -1, 1, (_, _1, _2) => 2) == 1);
            Assert.Throws<ArgumentException>(() => dictionary.AddOrUpdate(-1, 1, 1, (_, _1, _2) => 2));
            Assert.Throws<ArgumentException>(() => dictionary.AddOrUpdate(-1, -1, 1, (_, _1, _2) => 2));
            Assert.IsTrue(dictionary.ContainsKey(1, 1));
            Assert.IsTrue(dictionary.ContainsKey(1, -1));
            Assert.IsFalse(dictionary.ContainsKey(-1, 1));
            Assert.IsFalse(dictionary.ContainsKey(-1, -1));
            Assert.IsTrue(dictionary.AddOrUpdate(1, 1, 1, (_, _1, _2) => 2) == 2);
            Assert.IsTrue(dictionary.AddOrUpdate(1, -1, 1, (_, _1, _2) => 2) == 2);
            Assert.Throws<ArgumentException>(() => dictionary.AddOrUpdate(-1, 1, 1, (_, _1, _2) => 2));
            Assert.Throws<ArgumentException>(() => dictionary.AddOrUpdate(-1, -1, 1, (_, _1, _2) => 2));
            Assert.IsTrue(dictionary.ContainsKey(1, 1));
            Assert.IsTrue(dictionary.ContainsKey(1, -1));
            Assert.IsFalse(dictionary.ContainsKey(-1, 1));
            Assert.IsFalse(dictionary.ContainsKey(-1, -1));

            dictionary = new ConcurrentDoubleKeyDictionary<int, int, int>((k1, k2) => k1 > 0);
            Assert.IsTrue(dictionary.GetOrAdd(1, 1, (_, _1) => 1) == 1);
            Assert.IsTrue(dictionary.GetOrAdd(1, -1, (_, _1) => 1) == 1);
            Assert.Throws<ArgumentException>(() => dictionary.GetOrAdd(-1, 1, (_, _1) => 1));
            Assert.Throws<ArgumentException>(() => dictionary.GetOrAdd(-1, -1, (_, _1) => 1));
            Assert.IsTrue(dictionary.ContainsKey(1, 1));
            Assert.IsTrue(dictionary.ContainsKey(1, -1));
            Assert.IsFalse(dictionary.ContainsKey(-1, 1));
            Assert.IsFalse(dictionary.ContainsKey(-1, -1));
            Assert.IsTrue(dictionary.GetOrAdd(1, 1, (_, _1) => 1) == 1);
            Assert.IsTrue(dictionary.GetOrAdd(1, -1, (_, _1) => 1) == 1);
            Assert.Throws<ArgumentException>(() => dictionary.GetOrAdd(-1, 1, (_, _1) => 1));
            Assert.Throws<ArgumentException>(() => dictionary.GetOrAdd(-1, -1, (_, _1) => 1));
            Assert.IsTrue(dictionary.ContainsKey(1, 1));
            Assert.IsTrue(dictionary.ContainsKey(1, -1));
            Assert.IsFalse(dictionary.ContainsKey(-1, 1));
            Assert.IsFalse(dictionary.ContainsKey(-1, -1));
        }
        [Test]
        public void TestKey2Filter()
        {
            ConcurrentDoubleKeyDictionary<int, int, int> dictionary = new ConcurrentDoubleKeyDictionary<int, int, int>((k1, k2) => k2 > 0);
            Assert.IsTrue(dictionary.TryAdd(1, 1, 1));
            Assert.IsTrue(dictionary.TryAdd(-1, 1, 1));
            Assert.Throws<ArgumentException>(() => dictionary.TryAdd(1, -1, 1));
            Assert.Throws<ArgumentException>(() => dictionary.TryAdd(-1, -1, 1));
            Assert.IsTrue(dictionary.ContainsKey(1, 1));
            Assert.IsTrue(dictionary.ContainsKey(-1, 1));
            Assert.IsFalse(dictionary.ContainsKey(1, -1));
            Assert.IsFalse(dictionary.ContainsKey(-1, -1));

            dictionary = new ConcurrentDoubleKeyDictionary<int, int, int>((k1, k2) => k2 > 0);
            Assert.IsTrue(dictionary.AddOrUpdate(1, 1, 1, (_, _1, _2) => 2) == 1);
            Assert.IsTrue(dictionary.AddOrUpdate(-1, 1, 1, (_, _1, _2) => 2) == 1);
            Assert.Throws<ArgumentException>(() => dictionary.AddOrUpdate(1, -1, 1, (_, _1, _2) => 2));
            Assert.Throws<ArgumentException>(() => dictionary.AddOrUpdate(-1, -1, 1, (_, _1, _2) => 2));
            Assert.IsTrue(dictionary.ContainsKey(1, 1));
            Assert.IsTrue(dictionary.ContainsKey(-1, 1));
            Assert.IsFalse(dictionary.ContainsKey(1, -1));
            Assert.IsFalse(dictionary.ContainsKey(-1, -1));
            Assert.IsTrue(dictionary.AddOrUpdate(1, 1, 1, (_, _1, _2) => 2) == 2);
            Assert.IsTrue(dictionary.AddOrUpdate(-1, 1, 1, (_, _1, _2) => 2) == 2);
            Assert.Throws<ArgumentException>(() => dictionary.AddOrUpdate(1, -1, 1, (_, _1, _2) => 2));
            Assert.Throws<ArgumentException>(() => dictionary.AddOrUpdate(-1, -1, 1, (_, _1, _2) => 2));
            Assert.IsTrue(dictionary.ContainsKey(1, 1));
            Assert.IsTrue(dictionary.ContainsKey(-1, 1));
            Assert.IsFalse(dictionary.ContainsKey(1, -1));
            Assert.IsFalse(dictionary.ContainsKey(-1, -1));

            dictionary = new ConcurrentDoubleKeyDictionary<int, int, int>((k1, k2) => k2 > 0);
            Assert.IsTrue(dictionary.GetOrAdd(1, 1, (_, _1) => 1) == 1);
            Assert.IsTrue(dictionary.GetOrAdd(-1, 1, (_, _1) => 1) == 1);
            Assert.Throws<ArgumentException>(() => dictionary.GetOrAdd(1, -1, (_, _1) => 1));
            Assert.Throws<ArgumentException>(() => dictionary.GetOrAdd(-1, -1, (_, _1) => 1));
            Assert.IsTrue(dictionary.ContainsKey(1, 1));
            Assert.IsTrue(dictionary.ContainsKey(-1, 1));
            Assert.IsFalse(dictionary.ContainsKey(1, -1));
            Assert.IsFalse(dictionary.ContainsKey(-1, -1));
            Assert.IsTrue(dictionary.GetOrAdd(1, 1, (_, _1) => 1) == 1);
            Assert.IsTrue(dictionary.GetOrAdd(-1, 1, (_, _1) => 1) == 1);
            Assert.Throws<ArgumentException>(() => dictionary.GetOrAdd(1, -1, (_, _1) => 1));
            Assert.Throws<ArgumentException>(() => dictionary.GetOrAdd(-1, -1, (_, _1) => 1));
            Assert.IsTrue(dictionary.ContainsKey(1, 1));
            Assert.IsTrue(dictionary.ContainsKey(-1, 1));
            Assert.IsFalse(dictionary.ContainsKey(1, -1));
            Assert.IsFalse(dictionary.ContainsKey(-1, -1));
        }
        [Test]
        public void TestContainsKey()
        {
            ConcurrentDoubleKeyDictionary<int, int, int> dictionary = new ConcurrentDoubleKeyDictionary<int, int, int>();
            Assert.IsFalse(dictionary.ContainsKey(1));
            Assert.IsFalse(dictionary.ContainsKey(1, 2));

            Assert.IsTrue(dictionary.TryAdd(1, 2, 3));
            Assert.IsTrue(dictionary.ContainsKey(1));
            Assert.IsTrue(dictionary.ContainsKey(1, 2));

            Assert.IsTrue(dictionary.TryRemove(1, 2, out _));
            Assert.IsFalse(dictionary.ContainsKey(1));
            Assert.IsFalse(dictionary.ContainsKey(1, 2));
        }
        [Test]
        public void TestCount()
        {
            ConcurrentDoubleKeyDictionary<int, int, int> dictionary = new ConcurrentDoubleKeyDictionary<int, int, int>();
            Assert.Zero(dictionary.Count);
            Assert.Zero(dictionary.LongCount);

            Assert.IsTrue(dictionary.TryAdd(1, 2, 3));
            Assert.AreEqual(1, dictionary.Count);
            Assert.AreEqual(1, dictionary.LongCount);

            Assert.IsTrue(dictionary.TryRemove(1, 2, out _));
            Assert.Zero(dictionary.Count);
            Assert.Zero(dictionary.LongCount);

            // Consumes too much memory
            //dictionary = new ConcurrentDoubleKeyDictionary<ushort, ushort, ushort>();
            //for (long i = 0; i <= ((long)int.MaxValue) + 1; i++)
            //{
            //	Assert.True(dictionary.TryAdd((ushort)(i / ushort.MaxValue), (ushort)(i % ushort.MaxValue), 0));
            //}
            //Assert.Throws<OverflowException>(() => _ = dictionary.Count);
            //Assert.AreEqual(((long)int.MaxValue) + 1, dictionary.LongCount);
        }
        [Test]
        public void TestEquals()
        {
            ConcurrentDoubleKeyDictionary<int, int, int> dictionary = new ConcurrentDoubleKeyDictionary<int, int, int>();
            ConcurrentDoubleKeyDictionary<int, int, int> dictionary2 = new ConcurrentDoubleKeyDictionary<int, int, int>();

            Assert.AreEqual(dictionary, dictionary2);

            Assert.IsTrue(dictionary.TryAdd(1, 1, 1));
            Assert.AreNotEqual(dictionary, dictionary2);

            Assert.IsTrue(dictionary2.TryAdd(1, 1, 1));
            Assert.AreEqual(dictionary, dictionary2);

            for (int i = 2; i < 10; i++)
            {
                Assert.IsTrue(dictionary.TryAdd(1, i, i));
            }
            Assert.AreNotEqual(dictionary, dictionary2);

            for (int i = 2; i < 10; i++)
            {
                Assert.IsTrue(dictionary2.TryAdd(1, i, i));
            }
            Assert.AreEqual(dictionary, dictionary2);

            for (int i = 2; i < 10; i++)
            {
                Assert.IsTrue(dictionary.TryAdd(i, i, i));
            }
            Assert.AreNotEqual(dictionary, dictionary2);

            for (int i = 2; i < 10; i++)
            {
                Assert.IsTrue(dictionary2.TryAdd(i, i, i));
            }
            Assert.AreEqual(dictionary, dictionary2);
        }
    }
}