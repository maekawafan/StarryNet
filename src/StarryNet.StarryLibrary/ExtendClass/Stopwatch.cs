using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

public static class StopwatchEX
{
    public static double GetElapsedMilliSeconds(this Stopwatch stopWatch)
    {
        return (double)stopWatch.ElapsedTicks / Stopwatch.Frequency * 1000.0;
    }

    public static double GetElapsedSeconds(this Stopwatch stopWatch)
    {
        return (double)stopWatch.ElapsedTicks / Stopwatch.Frequency;
    }
}