using PackUtils.DictionaryUtils;
using System;
using Xunit;

namespace PackUtils.Test.DictionaryUtils
{
    public class DictionaryUtilityTest
    {
        [Fact]
        public void ToDictionary_Should_Throws_Exception_With_Null_Value()
        {
            // arrange / act / assert
            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() =>
                DictionaryUtility.ToDictionary(null));
            Assert.Equal("source", ex.ParamName);
        }

        [Fact]
        public void ToDictionary_Should_Return_A_Dictionary_With_Object_Values()
        {
            // arrange 
            PlainObject obj = new PlainObject
            {
                TestInt = 5,
                TestString = "test",
                TestNull = null
            };

            // act
            var dic = DictionaryUtility.ToDictionary(obj);

            // assert
            Assert.Equal(2, dic.Count);
            Assert.Equal("5", dic["TestInt"]);
            Assert.Equal("test", dic["TestString"]);
        }
    }

    class PlainObject
    {
        public string TestString { get; set; }

        public int TestInt { get; set; }

        public object TestNull { get; set; }
    }
}
