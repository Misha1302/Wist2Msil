﻿namespace Wist2Msil;

using System.Diagnostics;

public static class WistTimer
{
    public static long MeasureExecutionTime<T>(this Func<T> action, out T value)
    {
        var sw = Stopwatch.StartNew();
        value = action();
        sw.Stop();
        return sw.ElapsedMilliseconds;
    }
}