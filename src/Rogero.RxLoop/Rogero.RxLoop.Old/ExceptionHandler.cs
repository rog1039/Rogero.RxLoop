using System;

namespace Rogero.RxLoops
{
    public static class ExceptionHandler
    {
        public static void HandleException(Exception e)
        {
            RxLoopConfiguration.ExceptionHandler?.HandleException(e);
        }
    }
}