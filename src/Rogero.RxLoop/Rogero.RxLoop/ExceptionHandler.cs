using System;

namespace Rogero.RxLoops
{
    public static class ExceptionHandler
    {
        public static IExceptionHandler Handler { get; set; }

        public static void HandleException(Exception e)
        {
            if (Handler != null)
            {
                Handler.HandleException(e);
            }
        }
    }
}