using PackUtils.LinqUtils;
using NUnit.Framework;

namespace PackUtils.Test.LinqUtils
{
    [TestFixture]
    public class EnumerableExtensions_DistinctBy_Test
    {
        [Test]
        public void DistinctBy_filters_by_predicate()
        {
            var list = new[] { 1, 2, 3, 4, 5 };
            var result = list.DistinctBy(x => x % 2);

            Assert.AreEqual(new[] { 1, 2 }, result);
        }
    }
}
