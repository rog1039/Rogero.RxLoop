using System;
using System.Reactive.Disposables;
using System.Threading;

namespace Rogero.RxLoops
{
    public class RxLoopRedone
    {
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly Action<CancellationToken> _action;
        private CancellationToken _token;
        private static readonly object EmptyState = null;
        private bool _loopStarted;

        public TimeSpan DelayBetweenRuns { get; }

        public void StartLoop(CancellationToken token)
        {
            _token = token;
            _loopStarted = true;

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

        public void Pulse(CancellationToken token)
        {
            RunActionInternal(token);
        }

        public IDisposable PulseAfterDelay(CancellationToken token)
        {
            return _schedulerProvider.ThreadPool.Schedule(
                token,
                DelayBetweenRuns,
                (scheduler, state) =>
                {
                    RunActionInternal(state);
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