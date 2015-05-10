using System;

namespace Rogero.RxLoops
{
    public abstract class RxLoopInstanceBase : IDisposable
    {
        protected readonly RxLoop RxLoop;

        public RxLoopInstanceBase(TimeSpan delayBetweenRuns, ISchedulerProvider scheduler, string nameOfInstance)
        {
            RxLoop = new RxLoop(scheduler, Action, delayBetweenRuns, nameOfInstance);
        }

        protected void Start()
        {
            RxLoop.StartLoop();
        }
        
        public void RunOneIteration()
        {
            RxLoop.RunAction();
        }

        public void ScheduleOneIteration()
        {
            RxLoop.Schedule(RxLoop.RunAction);
        }

        public void Stop()
        {
            RxLoop.StopLoop();
        }

        public void Dispose()
        {
            Stop();
        }

        protected abstract void Action();
    }
}