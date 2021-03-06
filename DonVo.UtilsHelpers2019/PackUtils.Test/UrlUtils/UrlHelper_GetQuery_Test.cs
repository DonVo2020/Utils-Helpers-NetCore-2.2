﻿using System;
using System.Collections.Generic;
using PackUtils.UrlUtils;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace PackUtils.Test.UrlUtils
{
    [TestFixture]
    class UrlHelper_GetQuery_Test
    {
        [Test]
        public void GetQuery_accepts_anonymous_type()
        {
            var dict = new
            {
                A = 1,
                B = 2
            };
            Assert.AreEqual("A=1&B=2", UrlHelper.GetQuery(dict));
        }

        [Test]
        public void GetQuery_accepts_dictionary()
        {
            var dict = new Dictionary<string, object>
            {
                ["A"] = 1,
                ["B"] = 2
            };
            Assert.AreEqual("A=1&B=2", UrlHelper.GetQuery(dict));
        }

        [Test]
        public void GetQuery_accepts_JObject()
        {
            var dict = new JObject
            {
                ["A"] = 1,
                ["B"] = 2
            };
            Assert.AreEqual("A=1&B=2", UrlHelper.GetQuery(dict));
        }

        [Test]
        public void GetQuery_renders_array_elements()
        {
            var dict = new
            {
                A = new[] { 1, 2, 3 }
            };
            Assert.AreEqual("A=1&A=2&A=3", UrlHelper.GetQuery(dict));
        }

        [Test]
        public void GetQuery_renders_strings()
        {
            var dict = new
            {
                A = "hello",
                B = "world"
            };
            Assert.AreEqual("A=hello&B=world", UrlHelper.GetQuery(dict));
        }

        [Test]
        public void GetQuery_escapes_keys()
        {
            var dict = new JObject
            {
                ["A B"] = 1
            };
            Assert.AreEqual("A%20B=1", UrlHelper.GetQuery(dict));
        }

        [Test]
        public void GetQuery_escapes_values()
        {
            var dict = new JObject
            {
                ["A"] = "Hello/world"
            };
            Assert.AreEqual("A=Hello%2Fworld", UrlHelper.GetQuery(dict));
        }

        [Test]
        public void Combine_throws_ArgumentNullException_on_null_object()
        {
            Assert.Throws<ArgumentNullException>(() => UrlHelper.GetQuery(null));
        }

        [Test]
        public void Combine_throws_ArgumentNullException_on_null_props()
        {
            Assert.Throws<ArgumentNullException>(() => UrlHelper.GetQuery(null as IEnumerable<KeyValuePair<string, int>>));
        }
    }
}
