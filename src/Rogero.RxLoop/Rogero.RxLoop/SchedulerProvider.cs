using System.Reactive.Concurrency;

namespace Rogero.RxLoop
{
    public sealed class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler WpfScheduler { get; set; }
        public IScheduler CurrentThread { get { return CurrentThreadScheduler.Instance; } }
        public IScheduler Dispatcher { get { return DispatcherScheduler.Current; } }
        public IScheduler Immediate { get { return ImmediateScheduler.Instance; } }
        public IScheduler NewThread { get { return NewThreadScheduler.Default; } }
        public IScheduler ThreadPool { get { return ThreadPoolScheduler.Instance; } }
        public IScheduler TaskPool { get { return TaskPoolScheduler.Default; } }
    }
}