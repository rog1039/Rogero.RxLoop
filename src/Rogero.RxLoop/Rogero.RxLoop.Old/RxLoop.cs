﻿using System;
using System.Diagnostics;

namespace Rogero.RxLoops
{
    public class RxLoop : IDisposable
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly Action _action;
        private volatile bool _isEnabled = true;
        private IDisposable _actionScheduleDisposable;

        public TimeSpan DelayBetweenRuns { get; }
        public bool PrintDebugOutput { get; set; }
        public string Description { get; set; }
        public Guid LoopGuid { get; } = Guid.NewGuid();

        public RxLoop(ISchedulerProvider schedulerProvider, Action action, TimeSpan delayBetweenRuns, string description = default(string))
        {
            _schedulerProvider = schedulerProvider;
            _action = action;
            DelayBetweenRuns = delayBetweenRuns;
            Description = description;
            PrintDebugOutput = false;
        }

        public IDisposable StartLoop()
        {
            _isEnabled = true;
            ScheduleNextLoopIteration();
            return new StopRxTimerLoopDisposable(this);
        }

        private void ScheduleNextLoopIteration()
        {
            if (!_isEnabled)
                return;

            Schedule(RunActionAndSchedule);
        }

        internal void Schedule(Action action)
        {
            _actionScheduleDisposable = _schedulerProvider.ThreadPool.Schedule(
                (object)null,
                DelayBetweenRuns,
                (scheduler, state) => new DisposableActionWrapper(action));
        }

        private void RunActionAndSchedule()
        {
            RunAction();
            ScheduleNextLoopIteration();
        }

        public void RunOneIteration()
        {
            RunAction();
        }

        public void ScheduleOneIteration()
        {
            Schedule(RunAction);
        }

        internal void RunAction()
        {
            try
            {
                if (PrintDebugOutput)
                {
                    Debug.WriteLine("Running at time {0:yyyy.MM.dd | hh:mm:ss.ffff | zzz} | Ticks {2,3} | [{1}]",
                        _schedulerProvider.ThreadPool.Now,
                        Description,
                        _schedulerProvider.ThreadPool.Now.Ticks);
                }
                _action();
            }
            catch (Exception e)
            {
                ExceptionHandler.HandleException(e);
            }
        }

        public void Dispose()
        {
            StopLoop();
        }

        public void StopLoop()
        {
            _isEnabled = false;
            _actionScheduleDisposable.Dispose();
        }

        private class StopRxTimerLoopDisposable : IDisposable
        {
            private readonly RxLoop _rxLoop;

            public StopRxTimerLoopDisposable(RxLoop rxLoop)
            {
                _rxLoop = rxLoop;
            }

            public void Dispose()
            {
                _rxLoop.StopLoop();
            }
        }
    }
}
