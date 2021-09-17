using System;
using System.Collections.Generic;
using StarryNet.StarryLibrary;

namespace StarryNet.ServerModule
{
    public static class Schedule
    {
        internal struct Data
        {
            private DateTime createTime;
            internal DateTime runTime { get; private set; }
            internal double delay { get; private set; }
            internal int runCount { get; private set; }
            internal Action callback;

            internal Data(double delay, int runCount, Action callback)
            {
                this.createTime = ServerTime.now;
                this.runTime = createTime.AddSeconds(delay);
                this.delay = delay;
                this.runCount = runCount;
                this.callback = callback;
            }

            internal void SubtractCount()
            {
                if (runCount > 0)
                    runCount--;
            }

            internal void SetNextRunTime()
            {
                runTime = runTime.AddSeconds(delay);
            }
        }

        private static LinkedList<Data> schedules = new LinkedList<Data>();

        public static int scheduleCount
        {
            get
            {
                if (schedules == null)
                    return 0;
                return schedules.Count;
            }
        }

        public static void AddSchedule(double delay)
        {

        }

        public static void AddSchedule(double delay, Action callback, int runCount = 1)
        {
            AddData(new Data(delay, runCount, callback));
        }

        public static void AddLoopSchedule(double delay, Action callback)
        {
            AddData(new Data(delay, -1, callback));
        }

        private static void AddData(Data schedule)
        {
            if (schedules.IsEmpty())
            {
                schedules.AddFirst(schedule);
            }
            else
            {
                for (var current = schedules.First; ; current = current.Next)
                {
                    if (current == null)
                    {
                        schedules.AddLast(schedule);
                        break;
                    }
                    if (current.Value.runTime > schedule.runTime)
                    {
                        schedules.AddBefore(current, schedule);
                        break;
                    }
                }
            }
        }

        public static void ClearAll()
        {
            schedules.Clear();
        }

        public static void Execute()
        {
            for (var current = schedules.First; current != null; current = schedules.First)
            {
                try
                {
                    if (current.Value.runTime < ServerTime.now)
                    {
                        current.Value.callback();
                        PostExecuteSchedule();
                    }
                    else
                        break;
                }
                catch (Exception e)
                {
                    Log.Error("Schedule", $"스케쥴 실행 실패 - {e.Message}");
                    PostExecuteSchedule();
                }
            }
        }

        private static void PostExecuteSchedule()
        {
            Data data = schedules.First.Value;
            schedules.RemoveFirst();
            if (data.runCount > 0)
                data.SubtractCount();
            if (data.runCount != 0)
            {
                data.SetNextRunTime();
                AddData(data);
            }
        }
    }
}