using NUnit.Framework;
using PackUtils.LinqUtils;
using System.Collections.Generic;
using System.Linq;

namespace PackUtils.Test.LinqUtils
{
    [TestFixture]
    public class EnumerableExtensions_Recursive_Test
    {
        private SampleTreeNode[] GetTree()
        {
            return new[]
            {
                new SampleTreeNode(
                    1,
                    new SampleTreeNode(
                        2,
                        new SampleTreeNode(3),
                        new SampleTreeNode(4)
                    ),
                    new SampleTreeNode(
                        5,
                        new SampleTreeNode(
                            6,
                            new SampleTreeNode(7)
                        ),
                        new SampleTreeNode(8)
                    )
                ),
            };
        }

        [Test]
        public void SelectRecursive_selects_all_tree_nodes_in_depth_firt_order()
        {
            var tree = GetTree();
            var result = tree.SelectRecursively(x => x.Children).Select(x => x.Value);

            Assert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, result);
        }

        [Test]
        public void ApplyRecursive_visits_all_tree_nodes_in_depth_firt_order()
        {
            var tree = GetTree();
            var result = new List<int>();
            tree.ApplyRecursively(x => x.Children, x => result.Add(x.Value));

            Assert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, result);
        }
    }
}
