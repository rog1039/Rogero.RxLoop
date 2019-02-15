using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using Shouldly;
using Xunit;

namespace Rogero.Rx.Schedulers.Tests
{
    public class ISchedulerDelaysTestsUsingTestScheduler
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleDelayTest()
        {
            var testScheduler = new TestScheduler();
            var hasCompleted = false;

            Task.Run(async () =>
            {
                await testScheduler.Delay(TimeSpan.FromHours(1));
                hasCompleted = true;
            });

            Thread.Sleep(100);

            testScheduler.AdvanceBy(TimeSpan.FromMinutes(40));
            Thread.Sleep(100);
            hasCompleted.ShouldBeFalse();

            testScheduler.AdvanceBy(TimeSpan.FromMinutes(40));
            Thread.Sleep(100);
            hasCompleted.ShouldBeTrue();
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleSleepTest()
        {
            var testScheduler = new TestScheduler();
            var hasCompleted = false;

            Task.Run(() =>
            {
                testScheduler.Sleep(TimeSpan.FromHours(1));
                hasCompleted = true;
            });

            Thread.Sleep(100);

            testScheduler.AdvanceBy(TimeSpan.FromMinutes(40));
            Thread.Sleep(100);
            hasCompleted.ShouldBeFalse();

            testScheduler.AdvanceBy(TimeSpan.FromMinutes(40));
            Thread.Sleep(100);
            hasCompleted.ShouldBeTrue();
        }
    }

    public class ISchedulerDelayTestsUsingActualSchedulers
    {
        [Fact()]
        [Trait("Category", "Integration")]
        public void TestCurrentScheduler()
        {
            SimpleDelayTest(CurrentThreadScheduler.Instance);
            SimpleSleepTest(CurrentThreadScheduler.Instance);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void TestThreadPoolScheduler()
        {
            SimpleDelayTest(ThreadPoolScheduler.Instance);
            SimpleSleepTest(ThreadPoolScheduler.Instance);
        }

        public void SimpleDelayTest(IScheduler scheduler)
        {
            var hasCompleted = false;

            Task.Run(async () =>
            {
                await scheduler.Delay(TimeSpan.FromSeconds(2));
                hasCompleted = true;
            });

            Thread.Sleep(1500);
            hasCompleted.ShouldBeFalse();

            Thread.Sleep(1500);
            hasCompleted.ShouldBeTrue();
        }

        public void SimpleSleepTest(IScheduler scheduler)
        {
            var hasCompleted = false;

            Task.Run(() =>
            {
                scheduler.Sleep(TimeSpan.FromSeconds(2));
                hasCompleted = true;
            });

            Thread.Sleep(1500);
            hasCompleted.ShouldBeFalse();

            Thread.Sleep(1500);
            hasCompleted.ShouldBeTrue();
        }
    }
}