using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncUtils.Tasks
{
    public static class TaskUtils
    {
        public static async Task RunInParallel(IEnumerable<Func<Task>> operations, int maxDegreeOfParallelism)
        {
            List<Task> runningTasks = new List<Task>(maxDegreeOfParallelism);
            List<Exception> errors = new List<Exception>();

            foreach (Func<Task> operation in operations)
            {
                if (runningTasks.Count < maxDegreeOfParallelism)
                {
                    runningTasks.Add(operation());
                }
                else
                {
                    Task finishedTask = await Task.WhenAny(runningTasks);
                    runningTasks[runningTasks.IndexOf(finishedTask)] = operation();

                    if (finishedTask.IsFaulted)
                    {
                        errors.Add(finishedTask.Exception);
                    }
                }
            }

            while (runningTasks.Count > 0)
            {
                Task finishedTask = await Task.WhenAny(runningTasks);
                runningTasks.Remove(finishedTask);
                if (finishedTask.IsFaulted)
                {
                    errors.Add(finishedTask.Exception);
                }
            }

            if (errors.Count > 0)
            {
                throw new AggregateException(errors);
            }
        }
    }
}
