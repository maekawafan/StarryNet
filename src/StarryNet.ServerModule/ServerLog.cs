using log4net;

using System;

public class ServerLog : ConsoleLog
{
    private static ILog logger;

    public ServerLog(string logName)
    {
        log4net.GlobalContext.Properties["logFileName"] = logName;
        //log4net.Config.XmlConfigurator.Configure();

        logger = LogManager.GetLogger(logName);
    }

    public override void Write(string value)
    {
        base.Write(value);
        logger.Debug($"{LogTime} {value}");
    }
    public override void Write<T>(T value)
    {
        base.Write(value);
        logger.Debug($"{LogTime} {value}");
    }
    public override void Write(string title, string value)
    {
        base.Write(title, value);
        logger.Debug($"{LogTime} ({title}){value}");
    }
    public override void Write<T>(string title, T value)
    {
        base.Write(title, value);
        logger.Debug($"{LogTime} ({title}){value}");
    }
    public override void Error(string value)
    {
        base.Error(value);
        logger.Error($"{LogTime} Error - {value}");
    }
    public override void Error<T>(T value)
    {
        base.Error(value);
        logger.Error($"{LogTime} Error - {value}");
    }
    public override void Error(string title, string value)
    {
        base.Error(title, value);
        logger.Error($"{LogTime} ({title})Error - {value}");
    }
    public override void Error<T>(string title, T value)
    {
        base.Error(title, value);
        logger.Error($"{LogTime} ({title})Error - {value}");
    }
    public override void Info(string value)
    {
        base.Info(value);
        logger.Info($"{LogTime} Info - {value}");
    }
    public override void Info<T>(T value)
    {
        base.Info(value);
        logger.Info($"{LogTime} Info - {value}");
    }
    public override void Info(string title, string value)
    {
        base.Info(title, value);
        logger.Info($"{LogTime} ({title})Info - {value}");
    }
    public override void Info<T>(string title, T value)
    {
        base.Info(title, value);
        logger.Info($"{LogTime} ({title})Info - {value}");
    }
    public override void Debug(string value)
    {
        base.Debug(value);
    }
    public override void Debug<T>(T value)
    {
        base.Debug(value);
    }
    public override void Debug(string title, string value)
    {
        base.Debug(title, value);
    }
    public override void Debug<T>(string title, T value)
    {
        base.Debug(title, value);
    }
}