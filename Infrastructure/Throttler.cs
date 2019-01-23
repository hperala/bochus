using Core;
using System;

namespace Infrastructure
{
    public class Throttler : IThrottler
    {
        private DateTime lastAccessDate = DateTime.MinValue;

        public int MinIntervalMillis { get; set; }

        public bool IsAvailable(DateTime at)
        {
            CheckInvariant();

            var diff = at - lastAccessDate;
            return diff.TotalMilliseconds > MinIntervalMillis;
        }

        public void RequestCompleted(DateTime at)
        {
            CheckInvariant();

            lastAccessDate = at;
        }

        private void CheckInvariant()
        {
            if (!Invariant())
            {
                throw new InvalidOperationException();
            }
        }

        private bool Invariant()
        {
            return MinIntervalMillis > 0;
        }
    }
}