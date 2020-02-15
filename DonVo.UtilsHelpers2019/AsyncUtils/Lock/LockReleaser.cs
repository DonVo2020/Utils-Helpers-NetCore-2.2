using System;
using System.Diagnostics;
using System.Threading;

namespace AsyncUtils.Lock
{
    public sealed class LockReleaser : IDisposable
    {
        private AsyncLocker locker;

        public LockReleaser(AsyncLocker locker)
        {
            this.locker = locker;
        }

        public void Dispose()
        {
            try
            {
                AsyncLocker locker = Interlocked.Exchange(ref this.locker, null);

                if (locker == null)
                {
#if DEBUG
                    Debug.WriteLine($"Double free of lock detected. Maybe you disposed the {nameof(LockReleaser)} manualy inside using block?");
#endif
                }
                else
                {
                    locker?.Release();
                }
            }
            catch (NullReferenceException)
            {
#if DEBUG
                Debug.WriteLine($"Double free of lock detected. Maybe you disposed the {nameof(LockReleaser)} manualy inside using block?");
#endif
            }
        }
    }
}
