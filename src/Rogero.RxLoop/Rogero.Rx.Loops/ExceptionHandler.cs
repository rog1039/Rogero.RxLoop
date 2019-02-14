using System;

namespace Rogero.Rx.Loops
{
    public static class ExceptionHandler
    {
        public static void HandleException(Exception e)
        {
            RxLoopConfiguration.ExceptionHandler?.HandleException(e);
        }
    }
}