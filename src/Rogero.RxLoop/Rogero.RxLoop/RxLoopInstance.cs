using System;

namespace Rogero.RxLoop
{
    public abstract class RxLoopInstance : IDisposable
    {
        protected readonly RxLoop RxLoop;

        public RxLoopInstance(Action actionToPerform, TimeSpan delayBetweenRuns, ISchedulerProvider scheduler, string nameOfInstance)
        {
            RxLoop = new RxLoop(scheduler, actionToPerform, delayBetweenRuns, nameOfInstance);
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
    }
}