using System;

namespace StarryNet.StarryLibrary
{
    public static class ServerTime
    {
        static private bool isUsingUTC;
        static DateTime serverStartTime;
        static float serverFrame = 30.0f;
        static double serverTime = 0.0f;
        static float serverDelay;
        static bool isClientMode;

        static DateTime nextUpdateTime;

        public static DateTime _now;
        public static DateTime now
        {
            get
            {
                if (isClientMode)
                    return DateTime.Now;
                else
                    return _now;
            }
            set
            {
                _now = value;
            }
        }

        public static DateTime utcNow
        {
            get
            {
                if (isUsingUTC)
                    return DateTime.Now;
                else
                    return _now.Add(DateTime.UtcNow - DateTime.Now);
            }
        }

        public static void Initialize(float serverFrame, bool isClientMode = false)
        {
            ServerTime.serverFrame = serverFrame;
            ServerTime.isClientMode = isClientMode;
            serverDelay = 1.0f / serverFrame;
        }

        public static void StartTimer(bool isUsingUTC = true)
        {
            ServerTime.isUsingUTC = isUsingUTC;
            serverStartTime = isUsingUTC ? DateTime.UtcNow : DateTime.Now;
            now = serverStartTime;
            nextUpdateTime = serverStartTime.AddSeconds(serverDelay);
        }

        public static bool IsNeedUpdate()
        {
            return nextUpdateTime < (isUsingUTC ? DateTime.UtcNow : DateTime.Now);
        }

        public static double GetDeltaTime()
        {
            return (nextUpdateTime - now).TotalSeconds;
        }

        public static void Update(double deltaTime)
        {
            now = isUsingUTC ? DateTime.UtcNow : DateTime.Now;
            nextUpdateTime = now.AddSeconds(serverDelay);
            serverTime = deltaTime;
        }
    }
}