using System;

namespace Rogero.Rx.Loops
{
    public interface IExceptionHandler
    {
        void HandleException(Exception e);
    }
}