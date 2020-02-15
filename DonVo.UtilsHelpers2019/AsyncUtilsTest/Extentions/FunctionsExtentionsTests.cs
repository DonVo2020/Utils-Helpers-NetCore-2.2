using AsyncUtils.Extentions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace AsyncUtilsTest.Extentions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class FunctionsExtentionsTests
    {
        private class LimitedExceptionThrower
        {
            public int ThrowTimes { get; }

            private int runTimes = 0;

            public LimitedExceptionThrower(int throwTimes)
            {
                ThrowTimes = throwTimes;
            }

            public void Run()
            {
                runTimes++;

                if (runTimes <= ThrowTimes)
                {
                    throw new Exception();
                }
            }
            public T Run<T>(T result)
            {
                Run();
                return result;
            }
            public Task RunAsync()
            {
                return Task.Run(Run);
            }
            public async Task<T> RunAsync<T>(T result)
            {
                await RunAsync();
                return result;
            }
        }


        private static readonly Action THROW_EXCEPTION_FUNC = () => throw new Exception();


        [Test]
        public void TestInvokeAndRetry()
        {
            int retries = 3;

            FunctionsExtentions.InvokeAndRetry(() => { }, retries);
            FunctionsExtentions.InvokeAndRetry(new LimitedExceptionThrower(1).Run, retries);
            FunctionsExtentions.InvokeAndRetry(new LimitedExceptionThrower(retries - 1).Run, retries);
            Assert.AreEqual(retries, Assert.Throws<AggregateException>(() => FunctionsExtentions.InvokeAndRetry(THROW_EXCEPTION_FUNC, retries)).InnerExceptions.Count);
        }
        [Test]
        public void TestInvokeAndRetryWithResult()
        {
            int result = 1;
            int retries = 3;

            Assert.AreEqual(result, FunctionsExtentions.InvokeAndRetry(() => result, retries));

            LimitedExceptionThrower limitedExceptionThrower = new LimitedExceptionThrower(1);
            Assert.AreEqual(result, FunctionsExtentions.InvokeAndRetry(() => limitedExceptionThrower.Run(result), retries));

            limitedExceptionThrower = new LimitedExceptionThrower(retries - 1);
            Assert.AreEqual(result, FunctionsExtentions.InvokeAndRetry(() => limitedExceptionThrower.Run(result), retries));

            Assert.AreEqual(retries,
                            Assert.Throws<AggregateException>(() => FunctionsExtentions.InvokeAndRetry(() =>
                            {
                                THROW_EXCEPTION_FUNC();
                                return result;
                            },
                                                                                                        retries)).InnerExceptions.Count);
        }
        [Test]
        public async Task TestInvokeAndRetryAsync()
        {
            int retries = 3;

            await FunctionsExtentions.InvokeAndRetryAsync(() => Task.CompletedTask, retries);
            await FunctionsExtentions.InvokeAndRetryAsync(new LimitedExceptionThrower(1).RunAsync, retries);
            await FunctionsExtentions.InvokeAndRetryAsync(new LimitedExceptionThrower(retries - 1).RunAsync, retries);
            Assert.AreEqual(retries,
                            Assert.ThrowsAsync<AggregateException>(() => FunctionsExtentions.InvokeAndRetryAsync(() => Task.Run(THROW_EXCEPTION_FUNC),
                                                                                                                 retries)).InnerExceptions.Count);
        }
        [Test]
        public async Task TestInvokeAndRetryAsyncWithResult()
        {
            int result = 1;
            int retries = 3;

            await FunctionsExtentions.InvokeAndRetryAsync(() => Task.FromResult(result), retries);

            LimitedExceptionThrower limitedExceptionThrower = new LimitedExceptionThrower(1);
            Assert.AreEqual(result, await FunctionsExtentions.InvokeAndRetryAsync(() => limitedExceptionThrower.RunAsync(result), retries));

            limitedExceptionThrower = new LimitedExceptionThrower(retries - 1);
            Assert.AreEqual(result, await FunctionsExtentions.InvokeAndRetryAsync(() => limitedExceptionThrower.RunAsync(result), retries));

            Assert.AreEqual(retries,
                            Assert.ThrowsAsync<AggregateException>(() => FunctionsExtentions.InvokeAndRetryAsync(async () =>
                            {
                                THROW_EXCEPTION_FUNC();
                                return result;
                            },
                                                                                                                 retries)).InnerExceptions.Count);
        }
    }
}
