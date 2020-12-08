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
        public void SimpleDelayLoopTest()
        {
            var counter = 0;
            var scheduler = new TestScheduler();

            Task.Run(async () =>
            {
                while (true)
                {
                    await scheduler.Delay(TimeSpan.FromSeconds(2));
                    counter++;
                }
            });

            Thread.Sleep(100);
            scheduler.AdvanceBy(TimeSpan.FromSeconds(2.5));
            Thread.Sleep(100);
            scheduler.AdvanceBy(TimeSpan.FromSeconds(.1));
            Thread.Sleep(100);
            counter.ShouldBe(1);

            scheduler.AdvanceBy(TimeSpan.FromSeconds(2));
            Thread.Sleep(100);
            counter.ShouldBe(2);
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleSleepTest()
        {
            var testScheduler = new TestScheduler();
            var hasCompleted = false;

            Task.Run(() =>
            {
                testScheduler.ThreadSleep(TimeSpan.FromHours(1));
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
            SimpleDelayLoopTest(CurrentThreadScheduler.Instance);
        }

        [Fact()]
        [Trait("Category", "Integration")]
        public void TestThreadPoolScheduler()
        {
            SimpleDelayTest(ThreadPoolScheduler.Instance);
            SimpleSleepTest(ThreadPoolScheduler.Instance);
            SimpleDelayLoopTest(ThreadPoolScheduler.Instance);
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


        public void SimpleDelayLoopTest(IScheduler scheduler)
        {
            var counter = 0;

            Task.Run(async () =>
            {
                while (true)
                {
                    await scheduler.Delay(TimeSpan.FromSeconds(2));
                    counter++;
                }
            });

            Thread.Sleep(2000);
            counter.ShouldBe(1);

            Thread.Sleep(2000);
            counter.ShouldBe(2);
        }
        public void SimpleSleepTest(IScheduler scheduler)
        {
            var hasCompleted = false;

            Task.Run(() =>
            {
                scheduler.ThreadSleep(TimeSpan.FromSeconds(2));
                hasCompleted = true;
            });

            Thread.Sleep(1500);
            hasCompleted.ShouldBeFalse();

            Thread.Sleep(1500);
            hasCompleted.ShouldBeTrue();
        }
    }
}