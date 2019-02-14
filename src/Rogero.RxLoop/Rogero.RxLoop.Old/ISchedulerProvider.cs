using System.Reactive.Concurrency;

namespace Rogero.RxLoops
{
    public interface ISchedulerProvider
    {
        IScheduler WpfScheduler { get; set; }
        IScheduler CurrentThread { get; }
        IScheduler Dispatcher { get; }
        IScheduler Immediate { get; }
        IScheduler NewThread { get; }
        IScheduler ThreadPool { get; }
        IScheduler TaskPool { get; }
    }
}