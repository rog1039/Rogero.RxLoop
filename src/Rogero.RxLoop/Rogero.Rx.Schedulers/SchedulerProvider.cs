using System.Reactive.Concurrency;

namespace Rogero.Rx.Schedulers
{
    public sealed class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler Dispatcher { set; get; }
        public IScheduler CurrentThread { get { return CurrentThreadScheduler.Instance; } }
        public IScheduler Immediate { get { return ImmediateScheduler.Instance; } }
        public IScheduler NewThread { get { return NewThreadScheduler.Default; } }
        public IScheduler ThreadPool { get { return ThreadPoolScheduler.Instance; } }
        public IScheduler TaskPool { get { return TaskPoolScheduler.Default; } }
    }
}