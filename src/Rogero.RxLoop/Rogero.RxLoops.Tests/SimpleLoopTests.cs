using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Rogero.RxLoops.Tests
{
    public class SimpleLoopTests
    {
        private static int _callCount = 0;
        readonly Action _action = new Action(() => _callCount++);
        readonly TestSchedulers _testSchedulers = new TestSchedulers();

        [Fact()]
        [Trait("Category", "Instant")]
        public void AsTimePasses_TheLoopAction_ShouldBePerformedMultipleTimes()
        {
            
            "Given a loop that increments a call counter every 5 ticks"
                ._(() =>
                {
                    var loop = new RxLoop(_testSchedulers, _action, TimeSpan.FromTicks(5));
                    loop.StartLoop();
                });

            "Given an initial call count of 0"
                ._(() =>
                {
                    _callCount.Should().Be(0);
                });

            "Then the call count should be zero after 4 ticks"
                ._(() =>
                {
                    _testSchedulers.AdvanceAllBy(TimeSpan.FromTicks(4));
                    _callCount.Should().Be(0);
                });

            "Then the call count should be 1 after 5 ticks"
                ._(() =>
                {
                    _testSchedulers.AdvanceAllBy(TimeSpan.FromTicks(1));
                    _callCount.Should().Be(1);
                });

            "Then the call count should be 2 after 10 ticks"
                ._(() =>
                {
                    _testSchedulers.AdvanceAllBy(TimeSpan.FromTicks(5));
                    _callCount.Should().Be(2);
                });

            "Then the call count should be 10 after 50 ticks"
                ._(() =>
                {
                    _testSchedulers.AdvanceAllBy(TimeSpan.FromTicks(40));
                    _callCount.Should().Be(10);
                });
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void StopLoop_ShouldStop_FurtherProcessing()
        {
            RxLoop loop = null;

            "Given a loop that increments a call counter every 5 ticks"
                ._(() =>
                {
                    loop = new RxLoop(_testSchedulers, _action, TimeSpan.FromTicks(5));
                    loop.StartLoop();
                });

            "Given an initial call count of 0"
                ._(() =>
                {
                    _callCount.Should().Be(0);
                });

            "Given a  call count of 1 after 5 ticks"
                ._(() =>
                {
                    _testSchedulers.AdvanceAllBy(TimeSpan.FromTicks(5));
                    _callCount.Should().Be(1);
                });

            "Given we stop the loop"
                ._(() =>
                {
                    loop.StopLoop();
                });

            "When time passes"
                ._(() =>
                {
                    _testSchedulers.AdvanceAllBy(TimeSpan.FromTicks(50));
                });

            "Then no loop actions should be performed"
                ._(() =>
                {
                    _callCount.Should().Be(1);
                });
        }
    }
}
