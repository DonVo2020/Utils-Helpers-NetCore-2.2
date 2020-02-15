using System.Threading;
using NUnit.Framework;

namespace AsyncUtilsTest.TestUtils
{
    internal class SyncChecker
    {
        private int insideCounter = 0;

        public void Enter()
        {
            if (Interlocked.Increment(ref insideCounter) != 1)
            {
                throw new AssertionException("This is not thread safe!");
            }
        }

        public void Leave()
        {
            if (Interlocked.Decrement(ref insideCounter) != 0)
            {
                throw new AssertionException("This is not thread safe!");
            }
        }
    }
}
