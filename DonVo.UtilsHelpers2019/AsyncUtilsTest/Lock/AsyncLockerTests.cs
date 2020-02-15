using AsyncUtils.Lock;
using AsyncUtilsTest.TestUtils;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtilsTest.Lock
{
    [Parallelizable(ParallelScope.All)]
    [TestFixture]
    internal class AsyncLockerTests
    {
        [SetUp]
        public void SetUp()
        {
            ThreadPool.GetMinThreads(out int minWorkerThreads, out int minIoThreads);
            ThreadPool.SetMinThreads(Math.Max(4, minWorkerThreads), Math.Max(4, minIoThreads));
        }

        [Test]
        public async Task TestThreadSafeAsync()
        {
            SyncChecker syncChecker = new SyncChecker();

            using (AsyncLocker locker = new AsyncLocker())
            {
                Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                {
                    using (locker.Lock())
                    {
                        syncChecker.Enter();
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        syncChecker.Leave();
                    }
                })).ToArray();

                await Task.WhenAll(tasks);
            }
        }

        [Test]
        public async Task TestThreadSafe()
        {
            SyncChecker syncChecker = new SyncChecker();

            using (AsyncLocker locker = new AsyncLocker())
            {
                Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                {
                    using (locker.Lock())
                    {
                        syncChecker.Enter();
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        syncChecker.Leave();
                    }
                })).ToArray();

                await Task.WhenAll(tasks);
            }
        }

        [Test]
        public void TestThreadUnsafe()
        {
            SyncChecker syncChecker = new SyncChecker();

            Assert.ThrowsAsync<AssertionException>(async () =>
            {
                Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                {
                    syncChecker.Enter();
                    await Task.Delay(TimeSpan.FromSeconds(0.5));
                    syncChecker.Leave();
                })).ToArray();

                await Task.WhenAll(tasks);
            });
        }

        [Test]
        public void TestTimeoutAsync()
        {
            Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                using (AsyncLocker locker = new AsyncLocker())
                {
                    Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                    {
                        using (await locker.LockAsync(TimeSpan.FromSeconds(0.1)))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    })).ToArray();

                    await Task.WhenAll(tasks);
                }
            });
        }

        [Test]
        public void TestCancellationTokenAsync()
        {
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                using (AsyncLocker locker = new AsyncLocker())
                {
                    Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                    {
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.1));
                        using (await locker.LockAsync(cancellationTokenSource.Token))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    })).ToArray();

                    await Task.WhenAll(tasks);
                }
            });
        }

        [Test]
        public void TestTimeoutAndCancellationTokenAsync()
        {
            Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                using (AsyncLocker locker = new AsyncLocker())
                {
                    Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                    {
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.2));
                        using (await locker.LockAsync(TimeSpan.FromSeconds(0.1), cancellationTokenSource.Token))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    })).ToArray();

                    await Task.WhenAll(tasks);
                }
            });

            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                using (AsyncLocker locker = new AsyncLocker())
                {
                    Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                    {
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.1));
                        using (await locker.LockAsync(TimeSpan.FromSeconds(0.2), cancellationTokenSource.Token))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    })).ToArray();

                    await Task.WhenAll(tasks);
                }
            });
        }

        [Test]
        public void TestTimeout()
        {
            Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                using (AsyncLocker locker = new AsyncLocker())
                {
                    Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                    {
                        using (locker.Lock(TimeSpan.FromSeconds(0.1)))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    })).ToArray();

                    await Task.WhenAll(tasks);
                }
            });
        }

        [Test]
        public void TestCancellationToken()
        {
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                using (AsyncLocker locker = new AsyncLocker())
                {
                    Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                    {
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.1));
                        using (locker.Lock(cancellationTokenSource.Token))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    })).ToArray();

                    await Task.WhenAll(tasks);
                }
            });
        }

        [Test]
        public void TestTimeoutAndCancellationToken()
        {
            Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                using (AsyncLocker locker = new AsyncLocker())
                {
                    Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                    {
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                        using (locker.Lock(TimeSpan.FromSeconds(0.1), cancellationTokenSource.Token))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    })).ToArray();

                    await Task.WhenAll(tasks);
                }
            });

            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                using (AsyncLocker locker = new AsyncLocker())
                {
                    Task[] tasks = Enumerable.Range(0, 2).Select(x => Task.Run(async () =>
                    {
                        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(0.1));
                        using (locker.Lock(TimeSpan.FromSeconds(1), cancellationTokenSource.Token))
                        {
                            await Task.Delay(TimeSpan.FromSeconds(1));
                        }
                    })).ToArray();

                    await Task.WhenAll(tasks);
                }
            });
        }

        [Test]
        public void TestDoubleRelease()
        {
            using (AsyncLocker locker = new AsyncLocker())
            {
                using (LockReleaser l = locker.Lock())
                {
                    l.Dispose();
                }
            }
        }
    }
}
