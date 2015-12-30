using System;
using System.Reactive.Disposables;
using System.Threading;

namespace Rogero.RxLoops
{
    public class RxLoopCancellable
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly Action<CancellationToken> _action;

        public TimeSpan DelayBetweenRuns { get; }

        public RxLoopCancellable(ISchedulerProvider schedulerProvider, Action<CancellationToken> action, TimeSpan delayBetweenRuns)
        {
            _schedulerProvider = schedulerProvider;
            _action = action;
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
            _action(token);
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
}