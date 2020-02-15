using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtils.Extentions
{
    public static class TaskExtention
    {
        public static async Task SetTimeout(this Task task, TimeSpan timeout, CancellationTokenSource cancellationTokenSource = null)
        {
            using (CancellationTokenSource timeoutCancellationTokenSource = new CancellationTokenSource())
            {
                Task delayTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);
                await Task.WhenAny(task, delayTask);

                if (!task.IsCompleted)
                {
                    cancellationTokenSource?.Cancel();
                    throw new TimeoutException("Operation timed out");
                }
                else
                {
                    timeoutCancellationTokenSource.Cancel();
                    await task;
                }
            }
        }
        public static async Task<TResult> SetTimeout<TResult>(this Task<TResult> task, TimeSpan timeout, CancellationTokenSource cancellationTokenSource = null)
        {
            await (task as Task).SetTimeout(timeout, cancellationTokenSource);
            return await task;
        }
    }
}
