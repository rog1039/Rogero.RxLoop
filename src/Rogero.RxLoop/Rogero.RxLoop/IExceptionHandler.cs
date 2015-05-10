using System;

namespace Rogero.RxLoops
{
    public interface IExceptionHandler
    {
        void HandleException(Exception e);
    }
}