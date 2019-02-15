using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;

namespace Rogero.Rx.Schedulers
{
    public interface ISchedulerProvider
    {
        IScheduler CurrentThread { get; }
        IScheduler Dispatcher { get; }
        IScheduler Immediate { get; }
        IScheduler NewThread { get; }
        IScheduler ThreadPool { get; }
        IScheduler TaskPool { get; }
    }
}