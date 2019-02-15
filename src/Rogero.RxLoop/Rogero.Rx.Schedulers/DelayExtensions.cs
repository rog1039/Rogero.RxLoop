using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rogero.Rx.Schedulers
{
    public class VirtualDelay
    {
        private long                             _currentTime            = 0;
        private SortedList<long, BlockingRecord> _blockingRecordsSorted = new SortedList<long, BlockingRecord>();
        public StringBuilder _stringBuilder = new StringBuilder();

        public void Sleep(TimeSpan sleepTime)
        {
            var blockingRecord = new BlockingRecord(_currentTime, sleepTime);
            lock (_blockingRecordsSorted)
            {
                _blockingRecordsSorted.Add(blockingRecord.TimeBlockingShouldEnd, blockingRecord);
                WriteLogMessage("Sleep",$"New entry with sleeptime of {sleepTime}. Current time of {_currentTime}");
            }

            lock (blockingRecord.LockObject)
                while (blockingRecord.ContinueBlocking)
                    Monitor.Wait(blockingRecord.LockObject);
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
                    if (newCurrentTime >= topOfList.TimeBlockingShouldEnd)
                    {
                        WriteLogMessage("AdvanceTimeBy.Loop", "There is a BlockingRecord due so setting current time to it's EndTime");
                        _currentTime = topOfList.TimeBlockingShouldEnd;
                        WriteLogMessage($"AdvanceTimeBy.Loop", "Setting current time.");
                        lock (topOfList.LockObject)
                        {
                            WriteLogMessage("AdvanceTimeBy.Loop", $"Item off top has end time of {topOfList.TimeBlockingShouldEnd} so pulsing this record.");
                            topOfList.ContinueBlocking = false;
                            topOfList.Pulse();
                        }

                        _blockingRecordsSorted.RemoveAt(0);
                    }
                    else
                    {
                        WriteLogMessage("AdvanceTimeBy.Loop", $"Top of list is due later at {topOfList.TimeBlockingShouldEnd} than current time of {_currentTime}");
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
                _stringBuilder.AppendLine($"[{_currentTime, 10}] {area, 20}: {message}");
            }
        }
    }

    public class BlockingRecord
    {
        public long   TimeBlockingStarted   { get; set; }
        public long   TimeBlockingShouldEnd { get; set; }
        public object LockObject         { get; set; } = new object();
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