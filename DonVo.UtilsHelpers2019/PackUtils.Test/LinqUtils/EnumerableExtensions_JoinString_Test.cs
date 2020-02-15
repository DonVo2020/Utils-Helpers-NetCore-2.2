using PackUtils.LinqUtils;
using NUnit.Framework;

namespace PackUtils.Test.LinqUtils
{
    [TestFixture]
    public class EnumerableExtensions_JoinString_Test
    {
        [Test]
        public void JoinString_joins_with_separator()
        {
            Assert.AreEqual("A&B&C", new[] { "A", "B", "C" }.JoinString("&"));
        }

        [Test]
        public void JoinString_joins_with_empty_separator()
        {
            Assert.AreEqual("ABC", new[] { "A", "B", "C" }.JoinString(""));
        }

        [Test]
        public void JoinString_joins_with_null_separator()
        {
            Assert.AreEqual("ABC", new[] { "A", "B", "C" }.JoinString(null));
        }

        [Test]
        public void JoinString_returns_empty_string_on_empty_array()
        {
            Assert.AreEqual("", new string[0].JoinString(""));
        }
    }
}
