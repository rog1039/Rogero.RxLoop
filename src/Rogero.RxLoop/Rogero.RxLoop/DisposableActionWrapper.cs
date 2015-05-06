using System;

namespace Rogero.RxLoop
{
    /// <summary>
    /// This class only exists because we must return an object that implements IDisposable when scheduling 
    /// using the Rx IScheduler classes.  We simply want to run an action, so we wrap the action up in this
    /// object which is called by Rx at the appropriate time to run our action.
    /// </summary>
    public class DisposableActionWrapper : IDisposable
    {
        public DisposableActionWrapper(Action action)
        {
            action();
        }

        public void Dispose() { }
    }
}