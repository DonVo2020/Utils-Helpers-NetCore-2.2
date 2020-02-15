using NUnit.Framework;
using PackUtils.LinqUtils;
using PackUtils.RandomUtils;
using System;
using System.Linq;

namespace PackUtils.Test.LinqUtils
{
    [TestFixture]
    public class EnumerableExtensions_Random_Test
    {
        [Test]
        public void Shuffle_shuffles_array()
        {
            var src = Enumerable.Range(1, 100).ToList();

            RandomTestHelper.AtLeastOnce(() =>
            {
                var result = src.Shuffle().ToList();
                return result.SequenceEqual(src) == false
                       && result.OrderBy(x => x).SequenceEqual(src) == true;
            });
        }

        [Test]
        public void PickRandom_picks_elements()
        {
            var src = Enumerable.Range(1, 100).ToList();

            RandomTestHelper.AtLeastOnce(() =>
            {
                var a = src.PickRandom();
                var b = src.PickRandom();
                return src.Contains(a)
                       && src.Contains(b)
                       && a != b;
            });
        }
    }
}
