using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rogero.Rx.Schedulers
{
    public static class ISchedulerDelayExtensions
    {
        public static void Sleep(this IScheduler scheduler, TimeSpan sleepTime)
        {
            var lockObject = new object();
            var continueBlocking = true;

            scheduler.Schedule(sleepTime, () =>
            {
                lock (lockObject)
                {
                    continueBlocking = false;
                    Monitor.Pulse(lockObject);
                }
            });

            lock(lockObject)
                while (continueBlocking)
                    Monitor.Wait(lockObject);
        }

        public static Task Delay(this IScheduler scheduler, TimeSpan delayTime)
        {
            var taskCompletionSource = new TaskCompletionSource<Unit>();

            scheduler.Schedule(delayTime, ()=>
            {
                taskCompletionSource.SetResult(Unit.Default);
            });

            return taskCompletionSource.Task;
        }
    }
}
