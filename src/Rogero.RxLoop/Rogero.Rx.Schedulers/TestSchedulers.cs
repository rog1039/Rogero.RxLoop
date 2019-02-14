using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;

namespace Rogero.Rx.Schedulers
{
    public sealed class TestSchedulers : ISchedulerProvider
    {
        public TestScheduler CurrentThread { get { return _currentThread; } }
        public TestScheduler Dispatcher    { get { return _dispatcher; } }
        public TestScheduler Immediate     { get { return _immediate; } }
        public TestScheduler NewThread     { get { return _newThread; } }
        public TestScheduler ThreadPool    { get { return _threadPool; } }
        public TestScheduler TaskPool      { get { return _taskPool; } }
        
        private readonly TestScheduler _currentThread = new TestScheduler();
        private readonly TestScheduler _dispatcher = new TestScheduler();
        private readonly TestScheduler _immediate = new TestScheduler();
        private readonly TestScheduler _newThread = new TestScheduler();
        private readonly TestScheduler _threadPool = new TestScheduler();
        private readonly TestScheduler _taskPool = new TestScheduler();

        #region Explicit implementation of ISchedulerProvider
        
        IScheduler ISchedulerProvider.CurrentThread { get { return _currentThread; } }
        IScheduler ISchedulerProvider.Dispatcher { get { return _dispatcher; } }
        IScheduler ISchedulerProvider.Immediate { get { return _immediate; } }
        IScheduler ISchedulerProvider.NewThread { get { return _newThread; } }
        IScheduler ISchedulerProvider.ThreadPool { get { return _threadPool; } }
        IScheduler ISchedulerProvider.TaskPool { get { return _taskPool; } }
        
        #endregion

        public void StartAll()
        {
            CurrentThread.Start();
            Dispatcher.Start();
            Immediate.Start();
            NewThread.Start();
            ThreadPool.Start();
            TaskPool.Start();
        }
    }
}