using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Threading;

namespace Rogero.RxLoops
{
    public class RxLoopCancellable
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly Action<CancellationToken> _action;

        public string Description { get; }
        public TimeSpan DelayBetweenRuns { get; }
        public Guid LoopGuid { get; } = Guid.NewGuid();

        public RxLoopCancellable(ISchedulerProvider schedulerProvider, 
            Action<CancellationToken> action, 
            TimeSpan delayBetweenRuns,
            string description = default(string))
        {
            _schedulerProvider = schedulerProvider;
            _action = action;
            Description = description;
            DelayBetweenRuns = delayBetweenRuns;
        }

        public void StartLoop(CancellationToken token)
        {
            var loopState = new LoopState(token);
            DoLoop(loopState);
        }

        private void DoLoop(LoopState loopState)
        {
            if (loopState.CancellationToken.IsCancellationRequested)
                return;

            RunActionInternal(loopState.CancellationToken);
            ScheduleNextRun(loopState);
        }

        private void RunActionInternal(CancellationToken token)
        {
            RxLoopConfiguration.Trace?.Invoke(DebugOutput());

            _action(token);
        }

        private string DebugOutput()
        {
            return string.Format("Running at time {0:yyyy.MM.dd | hh:mm:ss.ffff | zzz} | Ticks {2,3} | [{1}] [{3}] ",
                                 _schedulerProvider.ThreadPool.Now,
                                 Description,
                                 _schedulerProvider.ThreadPool.Now.Ticks,
                                 LoopGuid);
        }

        private void ScheduleNextRun(LoopState loopState)
        {
            _schedulerProvider.ThreadPool.Schedule(
                loopState,
                DelayBetweenRuns,
                (scheduler, state) =>
                {
                    DoLoop(state);
                    return Disposable.Empty;
                }
                );
        }

        private class LoopState
        {
            public LoopState(CancellationToken token)
            {
                CancellationToken = token;
            }

            public CancellationToken CancellationToken { get; }
        }
    }

    public static class RxLoopConfiguration
    {
        public static Action<string> Trace { get; set; }
    }
}