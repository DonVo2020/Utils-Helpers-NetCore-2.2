using AsyncUtils.Tasks;
using AsyncUtilsTest.TestUtils;
using IO_Utils.Extentions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    internal class TaskUtilsTests
    {
        private static readonly TimeSpan RUNNING_TIME = TimeSpan.FromSeconds(0.1);
        private static readonly ICollection<ICollection<object>> ERROR_HANDLING_TEST_SOURCE = new object[][]
        {
            new object[]{ new bool[]{ true, true }, 1 },
            new object[]{ new bool[]{ true, true }, 2 },
            new object[]{ new bool[]{ true, true }, 5 },
            new object[]{ new bool[]{ false, true }, 1 },
            new object[]{ new bool[]{ false, true }, 2 },
            new object[]{ new bool[]{ false, true }, 5 },
            new object[]{ new bool[]{ true, false }, 1 },
            new object[]{ new bool[]{ true, false }, 2 },
            new object[]{ new bool[]{ true, false }, 5 },
            new object[]{ new bool[]{ true, false, true, false, true, false, true, false, true, false }, 1 },
            new object[]{ new bool[]{ true, false, true, false, true, false, true, false, true, false }, 10 },
            new object[]{ new bool[]{ true, false, true, false, true, false, true, false, true, false }, 50 },
            new object[]{ new bool[]{ false, true, false, true, false, true, false, true, false, true }, 1 },
            new object[]{ new bool[]{ false, true, false, true, false, true, false, true, false, true }, 10 },
            new object[]{ new bool[]{ false, true, false, true, false, true, false, true, false, true }, 50 },
            new object[]{ new bool[]{ true, true, true, true, true, false, false, false, false, false }, 1 },
            new object[]{ new bool[]{ true, true, true, true, true, false, false, false, false, false }, 10 },
            new object[]{ new bool[]{ true, true, true, true, true, false, false, false, false, false }, 50 },
            new object[]{ new bool[]{ false, false, false, false, false, true, true, true, true, true }, 1 },
            new object[]{ new bool[]{ false, false, false, false, false, true, true, true, true, true }, 10 },
            new object[]{ new bool[]{ false, false, false, false, false, true, true, true, true, true }, 50 },
        };


        [TestCase(100, 5)]
        [TestCase(10, 100)]
        [TestCase(10, 10)]
        public async Task TestRunInParallel_ParallelismDegree(int totalTasks, int maxDegreeOfParallelism)
        {
            VisitorsCounter visitorsCounter = new VisitorsCounter(maxDegreeOfParallelism);
            await TaskUtils.RunInParallel(((Func<Task>)(async () =>
            {
                visitorsCounter.Enter();
                await Task.Delay(RUNNING_TIME);
                visitorsCounter.Leave();
            })).Repeat(totalTasks), maxDegreeOfParallelism);

            Assert.Zero(visitorsCounter.CurrentVisitors);
            Assert.AreEqual(totalTasks, visitorsCounter.TotalVisitors);
            Assert.AreEqual(Math.Min(maxDegreeOfParallelism, totalTasks), visitorsCounter.MaxConcurrentVisitors);
        }

        [Test]
        [TestCaseSource(nameof(ERROR_HANDLING_TEST_SOURCE))]
        public void TestRunInParallel_ErrorHandling(ICollection<bool> whenToThrowException, int maxDegreeOfParallelism)
        {
            VisitorsCounter visitorsCounter = new VisitorsCounter(maxDegreeOfParallelism);
            IEnumerable<Func<Task>> opeartions = whenToThrowException.Select((toThrow) => (Func<Task>)(async () =>
            {
                visitorsCounter.Enter();
                await Task.Delay(RUNNING_TIME);
                visitorsCounter.Leave();
                if (toThrow)
                {
                    throw new ArgumentException();
                }
            }));
            int errorsNumber = whenToThrowException.Count((b) => b);

            Assert.AreEqual(errorsNumber,
                            Assert.ThrowsAsync<AggregateException>(() => TaskUtils.RunInParallel(opeartions, maxDegreeOfParallelism)).InnerExceptions.Count);
            Assert.Zero(visitorsCounter.CurrentVisitors);
            Assert.AreEqual(whenToThrowException.Count, visitorsCounter.TotalVisitors);
            Assert.AreEqual(Math.Min(maxDegreeOfParallelism, whenToThrowException.Count), visitorsCounter.MaxConcurrentVisitors);
        }
    }
}