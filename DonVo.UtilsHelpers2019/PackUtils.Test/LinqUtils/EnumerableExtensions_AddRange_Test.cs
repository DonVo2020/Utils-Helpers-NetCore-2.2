using System.Collections.Generic;
using PackUtils.LinqUtils;
using NUnit.Framework;

namespace PackUtils.Test.LinqUtils
{
    [TestFixture]
    public class EnumerableExtensions_AddRange_Test
    {
        [Test]
        public void AddRange_adds_values()
        {
            var list = new List<int> { 1, 2, 3 } as ICollection<int>;
            list.AddRange(new[] { 4, 5 });

            Assert.AreEqual(new[] { 1, 2, 3, 4, 5 }, list);
        }
    }
}
