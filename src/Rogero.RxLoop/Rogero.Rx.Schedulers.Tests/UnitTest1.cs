using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Rogero.Rx.Schedulers.Tests
{
    public class UnitTest1 : UnitTestBaseWithConsoleRedirection
    {
        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleOneSleepTest()
        {
            var hasCompleted = false;
            var virtualDelay = new VirtualDelay();
            Task.Run(() =>
            {
                virtualDelay.Sleep(TimeSpan.FromSeconds(30));
                hasCompleted = true;
            });

            Thread.Sleep(10);
            hasCompleted.ShouldBeFalse();

            virtualDelay.AdvanceTimeBy(TimeSpan.FromSeconds(29));
            Thread.Sleep(10);
            hasCompleted.ShouldBeFalse();

            virtualDelay.AdvanceTimeBy(TimeSpan.FromSeconds(1));
            Thread.Sleep(10);
            hasCompleted.ShouldBeTrue();
        }

        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleTwoSleepTest()
        {
            var virtualDelay = new VirtualDelay();

            try
            {
                var hasCompleted1 = false;
                var hasCompleted2 = false;

                Task.Run(() =>
                {
                    virtualDelay.Sleep(TimeSpan.FromSeconds(20));
                    hasCompleted1 = true;
                });
                Task.Run(() =>
                {
                    virtualDelay.Sleep(TimeSpan.FromSeconds(10));
                    hasCompleted2 = true;
                });

                Thread.Sleep(100);
                hasCompleted1.ShouldBeFalse();
                hasCompleted2.ShouldBeFalse();

                virtualDelay.AdvanceTimeBy(TimeSpan.FromSeconds(10));
                Thread.Sleep(1000);
                hasCompleted1.ShouldBeFalse();
                hasCompleted2.ShouldBeTrue();

                virtualDelay.AdvanceTimeBy(TimeSpan.FromSeconds(10));
                Thread.Sleep(100);
                hasCompleted1.ShouldBeTrue();
                hasCompleted2.ShouldBeTrue();

                virtualDelay.AdvanceTimeBy(TimeSpan.FromSeconds(5));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                Console.WriteLine(virtualDelay._stringBuilder.ToString());
            }
        }


        [Fact()]
        [Trait("Category", "Instant")]
        public void SimpleOneSleepWithUnevenAdvanceTimeTest()
        {
            var virtualDelay = new VirtualDelay();

            try
            {
                var hasCompleted = false;
                Task.Run(() =>
                {
                    virtualDelay.Sleep(TimeSpan.FromSeconds(10));
                    hasCompleted = true;
                });

                Thread.Sleep(10);
                hasCompleted.ShouldBeFalse();

                virtualDelay.AdvanceTimeBy(TimeSpan.FromSeconds(5));
                Thread.Sleep(10);
                hasCompleted.ShouldBeFalse();

                virtualDelay.AdvanceTimeBy(TimeSpan.FromSeconds(7));
                Thread.Sleep(10);
                hasCompleted.ShouldBeTrue();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                Console.WriteLine(virtualDelay._stringBuilder.ToString());
            }
        }

        public UnitTest1(ITestOutputHelper outputHelperHelper) : base(outputHelperHelper)
        {
        }
    }

    public class TestOutputHelperToTextWriterAdapter : TextWriter
    {
        ITestOutputHelper _output;

        public TestOutputHelperToTextWriterAdapter(ITestOutputHelper output)
        {
            _output = output;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }

        public override void WriteLine(string message)
        {
            _output.WriteLine(message);
        }

        public override void WriteLine(string format, params object[] args)
        {
            _output.WriteLine(format, args);
        }

        public override void Write(char value)
        {
        }

        public override void Write(string message)
        {
            _output.WriteLine(message);
        }
    }

    public class UnitTestBaseWithConsoleRedirection
    {
        protected ITestOutputHelper _outputHelper;

        public UnitTestBaseWithConsoleRedirection(ITestOutputHelper outputHelperHelper)
        {
            Console.SetOut(new TestOutputHelperToTextWriterAdapter(outputHelperHelper));
            _outputHelper = outputHelperHelper;
        }
    }
}