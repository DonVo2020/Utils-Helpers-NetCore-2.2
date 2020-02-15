using System.Collections.Generic;
using NUnit.Framework;
using PackUtils.DictionaryUtils;

namespace PackUtils.Test.DictionaryUtils
{
    /// <summary>
    /// Tests for DictionaryHelper.TryGetNullableValue.
    /// </summary>
    [TestFixture]
    public class DictionaryHelper_TryGetNullableValue_Test
    {
        [Test]
        public void TryGetNullableValue_returns_value_for_existing_keys()
        {
            var dict = new Dictionary<string, int>
            {
                ["hello"] = 1,
                ["a"] = 2
            };

            Assert.AreEqual(2, dict.TryGetNullableValue("a"));
        }

        [Test]
        public void TryGetNullableValue_returns_default_for_missing_keys()
        {
            var dict = new Dictionary<string, int>
            {
                ["hello"] = 1,
                ["a"] = 2
            };

            Assert.AreEqual(null, dict.TryGetNullableValue("bla"));
        }

        [Test]
        public void TryGetNullableValue_returns_default_for_null_as_single_elem()
        {
            var dict = new Dictionary<string, int>
            {
                ["hello"] = 1,
                ["a"] = 2
            };

            Assert.AreEqual(null, dict.TryGetNullableValue(null as string));
        }

        [Test]
        public void TryGetNullableValue_returns_default_for_null_as_array()
        {
            var dict = new Dictionary<string, int>
            {
                ["hello"] = 1,
                ["a"] = 2
            };

            Assert.AreEqual(null, dict.TryGetNullableValue(null as string[]));
        }

        [Test]
        public void TryGetNullableValue_accepts_list()
        {
            var dict = new Dictionary<int, int>
            {
                [1] = 13,
                [2] = 37
            };

            Assert.AreEqual(37, dict.TryGetNullableValue(3, 2, 1));
        }

        [Test]
        public void TryGetNullableValue_with_list_returns_null_for_missing_keys()
        {
            var dict = new Dictionary<int, int>
            {
                [1] = 1,
                [2] = 2
            };

            Assert.AreEqual(null, dict.TryGetNullableValue(4, 5));
        }

        [Test]
        public void TryGetNullableValue_with_list_skips_null_keys()
        {
            var dict = new Dictionary<string, int>
            {
                ["a"] = 1,
                ["b"] = 2
            };

            Assert.AreEqual(2, dict.TryGetNullableValue(null, "b"));
        }
    }
}
