﻿using NUnit.Framework;
using PackUtils.FormatUtils;
using System.Collections.Generic;

namespace PackUtils.Test.FormatUtils
{
    [TestFixture]
    public class EnumHelper_All_Test
    {
        [Test]
        public void GetEnumDescriptions_generic_returns_lookup_of_descriptions()
        {
            var result = EnumHelper.GetEnumDescriptions<SampleEnum>();

            var expected = new Dictionary<SampleEnum, string>
            {
                [SampleEnum.Hello] = "First value",
                [SampleEnum.World] = "Other value",
            };

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetEnumDescriptions_generic_falls_back_to_enum_keys()
        {
            var result = EnumHelper.GetEnumDescriptions<SampleEnum2>();

            var expected = new Dictionary<SampleEnum2, string>
            {
                [SampleEnum2.Hello] = "Hello",
                [SampleEnum2.World] = "World",
            };

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetEnumDescriptions_nongeneric_returns_lookup_of_descriptions()
        {
            var result = EnumHelper.GetEnumDescriptions(typeof(SampleEnum));

            var expected = new Dictionary<object, string>
            {
                [SampleEnum.Hello] = "First value",
                [SampleEnum.World] = "Other value",
            };

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetEnumDescriptions_nongeneric_falls_back_to_enum_keys()
        {
            var result = EnumHelper.GetEnumDescriptions(typeof(SampleEnum2));

            var expected = new Dictionary<object, string>
            {
                [SampleEnum2.Hello] = "Hello",
                [SampleEnum2.World] = "World",
            };

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetEnumDescription_returns_the_value()
        {
            Assert.AreEqual("First value", SampleEnum.Hello.GetEnumDescription());
        }

        [Test]
        public void GetEnumValues_returns_the_array()
        {
            Assert.AreEqual(new[] { SampleEnum.Hello, SampleEnum.World }, EnumHelper.GetEnumValues<SampleEnum>());
        }
    }
}
