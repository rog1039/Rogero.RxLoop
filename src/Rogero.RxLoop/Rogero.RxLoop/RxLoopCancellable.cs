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
            RxLoopConfiguration.Trace($"[{LoopGuid}] Starting loop.");
            var loopState = new LoopState(token);
            ScheduleNextRunImmediately(loopState);
        }

        private void ScheduleNextRunImmediately(LoopState loopState)
        {
            RxLoopConfiguration.Trace($"[{LoopGuid}] Scheduling immediate next run.");
            _schedulerProvider.ThreadPool.Schedule(
                loopState,
                TimeSpan.Zero,
                (scheduler, state) =>
                {
                    DoLoop(state);
                    return Disposable.Empty;
                }
                );
        }

        private void DoLoop(LoopState loopState)
        {
            RxLoopConfiguration.Trace($"[{LoopGuid}] DoLoop called");
            if (loopState.CancellationToken.IsCancellationRequested)
            {
                RxLoopConfiguration.Trace($"[{LoopGuid}] Loop cancelled via CancellationToken.");
                return;
            }

            RunActionInternal(loopState.CancellationToken);
            ScheduleNextRun(loopState);
        }

        private void RunActionInternal(CancellationToken token)
        {
            RxLoopConfiguration.Trace(DebugOutput());
            _action(token);
        }

        private string DebugOutput()
        {
            return string.Format("[{3}] Running at time {0:yyyy.MM.dd | hh:mm:ss.ffff | zzz} | Ticks {2,3} | [{1}]",
                                 _schedulerProvider.ThreadPool.Now,
                                 Description,
                                 _schedulerProvider.ThreadPool.Now.Ticks,
                                 LoopGuid);
        }

        private void ScheduleNextRun(LoopState loopState)
        {
            RxLoopConfiguration.Trace($"[{LoopGuid}] Scheduling next run.");
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

        private struct LoopState
        {
            public LoopState(CancellationToken token)
            {
                CancellationToken = token;
            }

            public CancellationToken CancellationToken { get; }
        }
    }
}