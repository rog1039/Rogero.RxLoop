using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;

namespace Rogero.RxLoop
{
    public sealed class TestSchedulers : ISchedulerProvider
    {
        private readonly TestScheduler _currentThread = new TestScheduler();
        private readonly TestScheduler _dispatcher = new TestScheduler();
        private readonly TestScheduler _immediate = new TestScheduler();
        private readonly TestScheduler _newThread = new TestScheduler();
        private readonly TestScheduler _threadPool = new TestScheduler();
        private TestScheduler _wpf = new TestScheduler();
        private readonly TestScheduler _taskPool = new TestScheduler();

        #region Explicit implementation of ISchedulerService
        IScheduler ISchedulerProvider.CurrentThread { get { return _currentThread; } }
        IScheduler ISchedulerProvider.Dispatcher { get { return _dispatcher; } }
        IScheduler ISchedulerProvider.Immediate { get { return _immediate; } }
        IScheduler ISchedulerProvider.NewThread { get { return _newThread; } }
        IScheduler ISchedulerProvider.ThreadPool { get { return _threadPool; } }
        IScheduler ISchedulerProvider.TaskPool { get { return _taskPool; } }
        IScheduler ISchedulerProvider.WpfScheduler
        {
            get { return _wpf; }
            set { _wpf = (TestScheduler)value; }
        }

        #endregion

        public TestScheduler CurrentThread { get { return _currentThread; } }
        public TestScheduler Dispatcher { get { return _dispatcher; } }
        public TestScheduler Immediate { get { return _immediate; } }
        public TestScheduler NewThread { get { return _newThread; } }
        public TestScheduler ThreadPool { get { return _threadPool; } }
        public TestScheduler TaskPool { get { return _taskPool; } }
        public TestScheduler WpfScheduler { get { return _wpf; } }

        public void StartAll()
        {
            CurrentThread.Start();
            Dispatcher.Start();
            Immediate.Start();
            NewThread.Start();
            ThreadPool.Start();
            TaskPool.Start();
            WpfScheduler.Start();
        }
    }
}