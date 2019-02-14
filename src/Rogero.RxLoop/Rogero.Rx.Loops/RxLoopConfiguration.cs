using System;

namespace Rogero.Rx.Loops
{
    public static class RxLoopConfiguration
    {
        public static Action<string> TraceHandler { get; set; }
        public static IExceptionHandler ExceptionHandler { get; set; }

        public static void Trace(string message)
        {
            TraceHandler?.Invoke(message);
        }
    }
}