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

        static DateTime nextUpdateTime;

        public static DateTime now;

        public static void Initialize(float serverFrame)
        {
            ServerTime.serverFrame = serverFrame;
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