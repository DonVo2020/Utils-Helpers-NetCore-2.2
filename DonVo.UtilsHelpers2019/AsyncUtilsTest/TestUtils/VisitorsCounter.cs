using System.Threading;
using NUnit.Framework;

namespace AsyncUtilsTest.TestUtils
{
    internal class VisitorsCounter
    {
        public int MaxVisitors { get; }
        public int MaxConcurrentVisitors { get; private set; }
        public int TotalVisitors => totalVisitors;
        public int CurrentVisitors => currentVisitors;

        private int currentVisitors = 0;
        private int totalVisitors = 0;
        private readonly object lockObject = new object();

        public VisitorsCounter(int maxVisitors)
        {
            MaxVisitors = maxVisitors;
        }

        public void Enter()
        {
            int newVisitorsNumber = Interlocked.Increment(ref currentVisitors);
            Interlocked.Increment(ref totalVisitors);
            lock (lockObject)
            {
                if (newVisitorsNumber > MaxConcurrentVisitors)
                {
                    MaxConcurrentVisitors = newVisitorsNumber;
                }
            }

            if (newVisitorsNumber > MaxVisitors)
            {
                throw new AssertionException("Too much visitors");
            }
        }

        public void Leave()
        {
            if (Interlocked.Decrement(ref currentVisitors) < 0)
            {
                throw new AssertionException("Too much leavers");
            }
        }
    }
}
