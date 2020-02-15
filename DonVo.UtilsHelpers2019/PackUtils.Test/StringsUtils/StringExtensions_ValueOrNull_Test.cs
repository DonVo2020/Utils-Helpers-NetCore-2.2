using NUnit.Framework;
using PackUtils.StringsUtils;

namespace PackUtils.Test.StringsUtils
{
    [TestFixture]
    public class StringExtensions_ValueOrNull_Test
    {
        [Test]
        public void ValueOrNull_returns_value_for_non_empty_string()
        {
            Assert.AreEqual("Test", "Test".ValueOrNull());
        }

        [Test]
        public void ValueOrNull_returns_value_for_whitespace_string_if_flag_is_enabled()
        {
            Assert.AreEqual("   ", "   ".ValueOrNull(allowWhitespace: true));
        }

        [Test]
        public void ValueOrNull_returns_null_for_whitespace_string()
        {
            Assert.IsNull("   ".ValueOrNull());
        }

        [Test]
        public void ValueOrNull_returns_null_for_empty_string()
        {
            Assert.IsNull("".ValueOrNull());
        }

        [Test]
        public void ValueOrNull_returns_null_for_null_string()
        {
            Assert.IsNull((null as string).ValueOrNull());
        }
    }
}
