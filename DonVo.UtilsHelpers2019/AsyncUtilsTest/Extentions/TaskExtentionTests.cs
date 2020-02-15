using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AsyncUtils.Extentions;

namespace AsyncUtilsTest.Extentions
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    internal class TaskExtentionTests
    {
        [Test]
        public async Task TestSetTimeout()
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            Task task = Task.Delay(TimeSpan.FromSeconds(1));
            Assert.ThrowsAsync<TimeoutException>(async () => await task.SetTimeout(TimeSpan.FromSeconds(0.1)));
            stopwatch.Stop();
            Console.WriteLine($"Running time: {stopwatch.Elapsed}");
            Assert.LessOrEqual(stopwatch.Elapsed, TimeSpan.FromSeconds(0.8));

            stopwatch.Reset();
            stopwatch.Start();
            task = Task.Delay(TimeSpan.FromSeconds(0.1));
            await task.SetTimeout(TimeSpan.FromSeconds(1));
            stopwatch.Stop();
            Console.WriteLine($"Running time: {stopwatch.Elapsed}");
            Assert.LessOrEqual(stopwatch.Elapsed, TimeSpan.FromSeconds(0.8));
        }
        [Test]
        public async Task TestSetTimeoutWithResult()
        {
            async Task<int> func(TimeSpan timeout)
            {
                await Task.Delay(timeout);
                return 1;
            }

            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            Task<int> task = func(TimeSpan.FromSeconds(1));
            Assert.ThrowsAsync<TimeoutException>(async () => await task.SetTimeout(TimeSpan.FromSeconds(0.1)));
            stopwatch.Stop();
            Console.WriteLine($"Running time: {stopwatch.Elapsed}");
            Assert.LessOrEqual(stopwatch.Elapsed, TimeSpan.FromSeconds(0.8));

            stopwatch.Reset();
            stopwatch.Start();
            task = func(TimeSpan.FromSeconds(0.1));
            Assert.AreEqual(1, await task.SetTimeout(TimeSpan.FromSeconds(1)));
            stopwatch.Stop();
            Console.WriteLine($"Running time: {stopwatch.Elapsed}");
            Assert.LessOrEqual(stopwatch.Elapsed, TimeSpan.FromSeconds(0.8));
        }
    }
}
