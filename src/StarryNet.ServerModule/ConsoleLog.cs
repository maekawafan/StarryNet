using StarryNet.StarryLibrary;
using System;

public class ConsoleLog : ILogSystem
{
    public static readonly object logLock = new object();
    public static string LogTime
    {
        get
        {
            return ServerTime.Now.ToString("[hh:mm:ss]");
        }
    }

    public virtual void Write(string value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.WriteLine($"{LogTime} {value}");
            }
#else
        Console.WriteLine($"{LogTime} {value}");
#endif
    }
    public virtual void Write<T>(T value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.WriteLine($"{LogTime} {value}");
            }
#else
        Console.WriteLine($"{LogTime} {value}");
#endif
    }
    public virtual void Write(string title, string value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(LogTime);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($" ({title})");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} ({title}){value}");
#endif
    }
    public virtual void Write<T>(string title, T value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(LogTime);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"({title}) ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} ({title}){value}");
#endif
    }
    public virtual void Error(string value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{LogTime} Error - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} Error - {value}");
#endif
    }
    public virtual void Error<T>(T value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{LogTime} Error - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} Error - {value}");
#endif
    }
    public virtual void Error(string title, string value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{LogTime}");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($" ({title})");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Error - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} ({title})Error - {value}");
#endif
    }
    public virtual void Error<T>(string title, T value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{LogTime}");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($" ({title})");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Error - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} ({title})Error - {value}");
#endif
    }
    public virtual void Info(string value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(LogTime);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($" Info - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} Info - {value}");
#endif
    }
    public virtual void Info<T>(T value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(LogTime);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($" Info - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} Info - {value}");
#endif
    }
    public virtual void Info(string title, string value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(LogTime);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"({title}) ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Info - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime}({title})Info - {value}");
#endif
    }
    public virtual void Info<T>(string title, T value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(LogTime);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"({title}) ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Info - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime}({title})Info - {value}");
#endif
    }
    public virtual void Debug(string value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{LogTime} Debug - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} Debug - {value}");
#endif
    }
    public virtual void Debug<T>(T value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{LogTime} Debug - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} Debug - {value}");
#endif
    }
    public virtual void Debug(string title, string value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{LogTime}");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($" ({title})");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Debug - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} {title}Debug - {value}");
#endif
    }
    public virtual void Debug<T>(string title, T value)
    {
#if COLOR_LOG
            lock (logLock)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{LogTime}");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($" ({title})");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Debug - ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(value);
                Console.ResetColor();
            }
#else
        Console.WriteLine($"{LogTime} {title}Debug - {value}");
#endif
    }

    public void ClearLine()
    {
        lock (logLock)
        {
            Console.Write(new string(' ', Console.BufferWidth));
        }
    }
}