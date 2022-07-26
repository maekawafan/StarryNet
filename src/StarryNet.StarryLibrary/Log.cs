namespace StarryNet.StarryLibrary
{
    public static class Log
    {
        private static ILogSystem logSystem;

        public static void Initialize(ILogSystem logSystem)
        {
            Log.logSystem = logSystem;
        }

        public static void Clear()
        {
            Log.logSystem = null;
        }

        public static bool IsInitialized()
        {
            return Log.logSystem != null;
        }

        public static void Write(string value)                  => logSystem.Write(value);
        public static void Write(string title, string value)    => logSystem.Write(title, value);
        public static void Write<T>(T value)                    => logSystem.Write(value);
        public static void Write<T>(string title, T value)      => logSystem.Write(title, value);
        public static void Error(string value)                  => logSystem.Error(value);
        public static void Error(string title, string value)    => logSystem.Error(title, value);
        public static void Error<T>(T value)                    => logSystem.Error(value);
        public static void Error<T>(string title, T value)      => logSystem.Error(title, value);
        public static void Info(string value)                   => logSystem.Info(value);
        public static void Info(string title, string value)     => logSystem.Info(title, value);
        public static void Info<T>(T value)                     => logSystem.Info(value);
        public static void Info<T>(string title, T value)       => logSystem.Info(title, value);
        public static void Debug(string value)                  => logSystem.Debug(value);
        public static void Debug(string title, string value)    => logSystem.Debug(title, value);
        public static void Debug<T>(T value)                    => logSystem.Debug(value);
        public static void Debug<T>(string title, T value)      => logSystem.Debug(title, value);
    }
}