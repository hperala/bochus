using System;

namespace Core
{
    public interface IThrottler
    {
        bool IsAvailable(DateTime at);

        void RequestCompleted(DateTime at);
    }
}
