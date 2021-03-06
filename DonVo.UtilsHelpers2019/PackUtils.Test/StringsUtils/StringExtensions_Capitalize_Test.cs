﻿using System;
using PackUtils.StringsUtils;
using NUnit.Framework;

namespace PackUtils.Test.StringsUtils
{
    [TestFixture]
    public class StringExtensions_Capitalize_Test
    {
        [Test]
        public void Capitalize_converts_first_character_to_uppercase()
        {
            Assert.AreEqual("Hello", "hello".Capitalize());
        }

        [Test]
        public void Capitalize_converts_first_character_to_uppercase_in_russian()
        {
            Assert.AreEqual("Привет", "привет".Capitalize());
        }

        [Test]
        public void Capitalize_does_not_affect_special_chars()
        {
            Assert.AreEqual("123", "123".Capitalize());
        }

        [Test]
        public void Capitalize_does_not_affect_already_uppercase_chars()
        {
            Assert.AreEqual("TEST", "TEST".Capitalize());
        }

        [Test]
        public void Capitalize_does_not_affect_empty_strings()
        {
            Assert.AreEqual("", "".Capitalize());
        }

        [Test]
        public void Capitalize_throws_on_null_string()
        {
            Assert.Throws<ArgumentNullException>(() => (null as string).Capitalize());
        }

        [Test]
        public void Capitalize_throws_on_null_culture()
        {
            Assert.Throws<ArgumentNullException>(() => "hello".Capitalize(null));
        }
    }
}
