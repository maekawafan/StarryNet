using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerModule
{
    public class ServerProfiler
    {
        Stopwatch stopwatch = new Stopwatch();
        private double serverUpdateDelay;
        public int captureCount { get; private set; }
        public double processTime = 0.0;

        public ServerProfiler(double serverUpdateDelay)
        {
            this.serverUpdateDelay = serverUpdateDelay;
        }

        internal void CaptureStart()
        {
            stopwatch.Restart();
        }

        internal void CaptureEnd()
        {
            processTime += stopwatch.GetElapsedMilliSeconds();
            captureCount++;
        }

        public string Report()
        {
            string result = $"Average Process Time : {ProcessAverageMillisecond().ToString("0.0000")}ms [{(GetOverworkRate() * 100.0).ToString("0.00")}%]";
            Reset();
            return result;
        }

        public double GetOverworkRate()
        {
            return ProcessAverageSecond() / serverUpdateDelay;
        }

        public double ProcessAverageSecond()
        {
            if (captureCount == 0)
                return 0.0;
            return processTime / captureCount / 1000.0;
        }

        public double ProcessAverageMillisecond()
        {
            return processTime / captureCount;
        }

        public void Reset()
        {
            processTime = 0.0;
            captureCount = 0;
        }

        public CaptureScope MakeCaptureScope()
        {
            return new CaptureScope(this);
        }

        public class CaptureScope : IDisposable
        {
            ServerProfiler owner;

            internal CaptureScope(ServerProfiler owner)
            {
                this.owner = owner;
                owner.CaptureStart();
            }

            public void Dispose()
            {
                owner.CaptureEnd();
            }
        }
    }
}
