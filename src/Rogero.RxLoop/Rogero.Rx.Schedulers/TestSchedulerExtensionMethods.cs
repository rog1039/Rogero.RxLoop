using System;
using Microsoft.Reactive.Testing;

namespace Rogero.Rx.Schedulers
{
    public static class TestSchedulerExtensionMethods
    {
        public static void AdvanceBy(this TestScheduler scheduler, TimeSpan timespan)
        {
            scheduler.AdvanceBy(timespan.Ticks);
        }

        public static void AdvanceAllBy(this TestSchedulers scheduler, TimeSpan timespan)
        {
            scheduler.CurrentThread.AdvanceBy(timespan.Ticks);
            scheduler.Dispatcher.AdvanceBy(timespan.Ticks);
            scheduler.Immediate.AdvanceBy(timespan.Ticks);
            scheduler.NewThread.AdvanceBy(timespan.Ticks);
            scheduler.ThreadPool.AdvanceBy(timespan.Ticks);
            scheduler.TaskPool.AdvanceBy(timespan.Ticks);
        }
    }
}