using System;

namespace Rogero.RxLoop
{
    public interface IExceptionHandler
    {
        void HandleException(Exception e);
    }
}