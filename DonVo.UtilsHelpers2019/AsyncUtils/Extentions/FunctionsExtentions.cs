using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtils.Extentions
{
    public static class FunctionsExtentions
    {
        public static T InvokeAndRetry<T>(this Func<T> func,
                                          int maxRetries,
                                          Predicate<Exception> ignoredExceptionsPredicate = null,
                                          CancellationToken cancellationToken = default)
        {
            List<Exception> exceptions = null;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return func();
                }
                catch (Exception e) when (ignoredExceptionsPredicate == null || ignoredExceptionsPredicate(e))
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>(maxRetries);
                    }
                    exceptions.Add(e);
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            throw new AggregateException($"Errors while trying to invoke function with {maxRetries} retries", exceptions.ToArray());
        }
        public static void InvokeAndRetry(this Action func,
                                          int maxRetries,
                                          Predicate<Exception> ignoredExceptionsPredicate = null,
                                          CancellationToken cancellationToken = default)
        {
            InvokeAndRetry((Func<object>)(() =>
            {
                func();
                return null;
            }), maxRetries, ignoredExceptionsPredicate, cancellationToken);
        }
        public static async Task<T> InvokeAndRetryAsync<T>(this Func<Task<T>> func,
                                                           int maxRetries,
                                                           Predicate<Exception> ignoredExceptionsPredicate = null,
                                                           CancellationToken cancellationToken = default)
        {
            List<Exception> exceptions = null;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return await func();
                }
                catch (Exception e) when (ignoredExceptionsPredicate == null || ignoredExceptionsPredicate(e))
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>(maxRetries);
                    }
                    exceptions.Add(e);
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            throw new AggregateException($"Errors while trying to invoke function with {maxRetries} retries", exceptions.ToArray());
        }
        public static Task InvokeAndRetryAsync(this Func<Task> func,
                                               int maxRetries,
                                               Predicate<Exception> ignoredExceptionsPredicate = null,
                                               CancellationToken cancellationToken = default)
        {
            return InvokeAndRetryAsync((Func<Task<object>>)(async () =>
            {
                await func();
                return null;
            }), maxRetries, ignoredExceptionsPredicate, cancellationToken);
        }
    }
}
