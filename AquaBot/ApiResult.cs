using System;
using System.Threading;

namespace AquaBot
{
    internal class ApiResult : IAsyncResult
    {
        internal ApiObject apiObject;
        internal ManualResetEvent manualEvent;

        public WaitHandle AsyncWaitHandle { get { return this.manualEvent; } }
        public bool IsCompleted { get; internal set; }
        public object AsyncState { get; internal set; }
        public bool CompletedSynchronously { get; internal set; }
    }
}