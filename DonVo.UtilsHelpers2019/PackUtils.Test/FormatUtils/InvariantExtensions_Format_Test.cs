using NUnit.Framework;
using PackUtils.FormatUtils;

namespace PackUtils.Test.FormatUtils
{
    /// <summary>
    /// InvariantExtensions helpers.
    /// </summary>
    [TestFixture]
    public class InvariantExtensions_Format_Test
    {
        [Test]
        public void ToInvariantString_formats_float()
        {
            Assert.AreEqual("1.337", 1.337f.ToInvariantString());
        }

        [Test]
        public void ToInvariantString_formats_double()
        {
            Assert.AreEqual("1.337", 1.337.ToInvariantString());
        }
    }
}
