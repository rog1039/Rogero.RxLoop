using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rogero.Rx.Schedulers
{
    public abstract class ScheduledAction
    {
        public long TimeActionWasQueued   { get; set; }
        public long TimeActionIsDue { get; set; }
        
        public ScheduledAction(long timeActionWasQueued, TimeSpan sleepTime)
        {
            TimeActionWasQueued   = timeActionWasQueued;
            TimeActionIsDue = timeActionWasQueued + sleepTime.Ticks;
        }
        
        public abstract void Execute();
    }

    public class ScheduledActionSleep : ScheduledAction
    {        
        public volatile bool ContinueBlocking = true;

        public override void Execute()
        {
            ContinueBlocking = false;
            Monitor.Pulse(this);
        }

        public ScheduledActionSleep(long timeActionWasQueued, TimeSpan sleepTime) : base(timeActionWasQueued, sleepTime)
        {
        }
    }

    public class ScheduledActionDelay : ScheduledAction
    {
        private TaskCompletionSource<Unit> _taskCompletionSource = new TaskCompletionSource<Unit>();

        public Task Task => _taskCompletionSource.Task;
        
        public override void Execute()
        {
            _taskCompletionSource.SetResult(Unit.Default);
        }

        public ScheduledActionDelay(long timeActionWasQueued, TimeSpan sleepTime) : base(timeActionWasQueued, sleepTime)
        {
        }
    }

    public class VirtualDelay
    {
        private long                             _currentTime           = 0;
        private SortedList<long, ScheduledAction> _blockingRecordsSorted = new SortedList<long, ScheduledAction>();
        public  StringBuilder                    _stringBuilder         = new StringBuilder();

        public void Sleep(TimeSpan sleepTime)
        {
            var blockingRecord = new ScheduledActionSleep(_currentTime, sleepTime);
            lock (_blockingRecordsSorted)
            {
                _blockingRecordsSorted.Add(blockingRecord.TimeActionIsDue, blockingRecord);
                WriteLogMessage("Sleep", $"New entry with sleeptime of {sleepTime}. Current time of {_currentTime}");
            }

            lock (blockingRecord)
                while (blockingRecord.ContinueBlocking)
                    Monitor.Wait(blockingRecord);
        }

        public Task Delay(TimeSpan sleepTime)
        {
            var blockingRecord = new ScheduledActionDelay(_currentTime, sleepTime);
            lock (_blockingRecordsSorted)
            {
                _blockingRecordsSorted.Add(blockingRecord.TimeActionIsDue, blockingRecord);
                WriteLogMessage("Delay", $"New entry with delay of {sleepTime}. Current time of {_currentTime}");
            }

            return blockingRecord.Task;
        }

        public void AdvanceTimeBy(TimeSpan advanceTimeAmount)
        {
            WriteLogMessage("AdvanceTimeBy.Start", $"Starting to advance time by {advanceTimeAmount}");
            lock (_blockingRecordsSorted)
            {
                var newCurrentTime = _currentTime + advanceTimeAmount.Ticks;
                if (_blockingRecordsSorted.Count == 0)
                {
                    WriteLogMessage("AdvanceTimeBy.Start", "Time advanced straight away since there are no BlockingRecords.");
                    _currentTime = newCurrentTime;
                    return;
                }

                while (true)
                {
                    if (_blockingRecordsSorted.Count == 0)
                    {
                        WriteLogMessage("AdvanceTimeBy.Loop", $"Exiting because _blockingRecords.Count == 0");
                        break;
                    }

                    var topOfList = _blockingRecordsSorted.First().Value;
                    if (newCurrentTime >= topOfList.TimeActionIsDue)
                    {
                        WriteLogMessage("AdvanceTimeBy.Loop", "There is a BlockingRecord due so setting current time to it's EndTime");
                        _currentTime = topOfList.TimeActionIsDue;
                        WriteLogMessage($"AdvanceTimeBy.Loop", "Setting current time.");
                        lock (topOfList)
                        {
                            WriteLogMessage("AdvanceTimeBy.Loop", $"Item off top has end time of {topOfList.TimeActionIsDue} so pulsing this record.");
                            topOfList.Execute();
                        }

                        _blockingRecordsSorted.RemoveAt(0);
                    }
                    else
                    {
                        WriteLogMessage("AdvanceTimeBy.Loop", $"Top of list is due later at {topOfList.TimeActionIsDue} than current time of {_currentTime}");
                        break;
                    }
                }

                WriteLogMessage("AdvanceTimeBy.End", $"Finished loop so setting the current time of {_currentTime} to {newCurrentTime}\r\n");
                _currentTime = newCurrentTime;
            }
        }

        private void WriteLogMessage(string area, string message)
        {
            lock (_blockingRecordsSorted)
            {
                _stringBuilder.AppendLine($"[{_currentTime,10}] {area,20}: {message}");
            }
        }
    }

    public class BlockingRecord
    {
        public          long   TimeBlockingStarted   { get; set; }
        public          long   TimeBlockingShouldEnd { get; set; }
        public          object LockObject            { get; set; } = new object();
        public volatile bool   ContinueBlocking = true;

        public BlockingRecord(long timeBlockingStarted, TimeSpan sleepTime)
        {
            TimeBlockingStarted   = timeBlockingStarted;
            TimeBlockingShouldEnd = timeBlockingStarted + sleepTime.Ticks;
        }

        public void Pulse()
        {
            Monitor.Pulse(LockObject);
        }
    }
}