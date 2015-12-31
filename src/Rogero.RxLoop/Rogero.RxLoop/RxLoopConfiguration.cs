using System;

namespace Rogero.RxLoops
{
    public static class RxLoopConfiguration
    {
        public static Action<string> Trace { get; set; }
        public static IExceptionHandler ExceptionHandler { get; set; }
    }
}