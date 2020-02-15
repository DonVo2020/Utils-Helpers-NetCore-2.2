using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncUtils.Lock
{
    public sealed class AsyncLocker : IDisposable
    {
        private readonly SemaphoreSlim semaphore;

        public AsyncLocker(int concurrentTasks = 1)
        {
            semaphore = new SemaphoreSlim(concurrentTasks, concurrentTasks);
        }

        public LockReleaser Lock()
        {
            semaphore.Wait();
            return new LockReleaser(this);
        }
        public LockReleaser Lock(TimeSpan timeout)
        {
            if (semaphore.Wait(timeout))
            {
                return new LockReleaser(this);
            }
            else
            {
                throw new TimeoutException("Timeout while trying to acquire lock");
            }
        }
        public LockReleaser Lock(CancellationToken cancellationToken)
        {
            semaphore.Wait(cancellationToken);
            return new LockReleaser(this);
        }
        public LockReleaser Lock(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (semaphore.Wait(timeout, cancellationToken))
            {
                return new LockReleaser(this);
            }
            else
            {
                throw new TimeoutException("Timeout while trying to acquire lock");
            }
        }
        public async Task<LockReleaser> LockAsync()
        {
            await semaphore.WaitAsync();
            return new LockReleaser(this);
        }
        public async Task<LockReleaser> LockAsync(TimeSpan timeout)
        {
            if (await semaphore.WaitAsync(timeout))
            {
                return new LockReleaser(this);
            }
            else
            {
                throw new TimeoutException("Timeout while trying to acquire lock");
            }
        }
        public async Task<LockReleaser> LockAsync(CancellationToken cancellationToken)
        {
            await semaphore.WaitAsync(cancellationToken);
            return new LockReleaser(this);
        }
        public async Task<LockReleaser> LockAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (await semaphore.WaitAsync(timeout, cancellationToken))
            {
                return new LockReleaser(this);
            }
            else
            {
                throw new TimeoutException("Timeout while trying to acquire lock");
            }
        }

        public bool Release()
        {
            try
            {
                semaphore.Release();
                return true;
            }
            catch (SemaphoreFullException)
            {
                return false;
            }
        }

        public void Dispose()
        {
            semaphore.Dispose();
        }
    }
}
