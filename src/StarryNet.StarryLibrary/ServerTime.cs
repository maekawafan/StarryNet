using System;

namespace StarryNet.StarryLibrary
{
    public static class ServerTime
    {
        public const double syncFailTime = 2.0;
        public const double resyncTime = 10.0;
        public const double additionalTimeRate = 0.2;
        static public double syncTolerance { get; private set; }
        static public double lastSyncRequestTime;

        static public double timeGap { get; private set; }

        static public double TrueTime => SyncTime + timeGap;

        static private bool isClientMode = false;
        static private bool isUsingUTC = false;
        static DateTime serverStartTime;
        static float serverFrame = 60.0f;
        static private double syncTime = 0.0f;
        public static double ServerDelay { get { return 1.0 / serverFrame; } }

        static DateTime nextUpdateTime;

        private static DateTime now;
        public static DateTime Now
        {
            get
            {
                if (isClientMode)
                    return DateTime.Now;
                else
                    return now;
            }
            set
            {
                now = value;
            }
        }

        public static DateTime utcNow
        {
            get
            {
                if (isUsingUTC)
                    return DateTime.Now;
                else
                    return now.Add(DateTime.UtcNow - DateTime.Now);
            }
        }

        public static double SyncTime { get { return syncTime; } }
        public static float SyncFloatTime { get { return (float)syncTime; } }
        public static double DeltaTime { get { return (nextUpdateTime - Now).TotalSeconds; } }
        public static float DeltaFloatTime { get { return (float)DeltaTime; } }

        public static void Initialize(float serverFrame, bool isClientMode)
        {
            ServerTime.serverFrame = serverFrame;
            ServerTime.isClientMode = isClientMode;
            if (!isClientMode)
            {
                syncTolerance = 0.050;
            }
        }

        public static void Clear()
        {
            syncTime = 0.0f;
            serverStartTime = DateTime.MinValue;
        }

        public static void StartTimer(bool isUsingUTC = true)
        {
            ServerTime.isUsingUTC = isUsingUTC;
            serverStartTime = isUsingUTC ? DateTime.UtcNow : DateTime.Now;
            Now = serverStartTime;
            nextUpdateTime = serverStartTime.AddSeconds(ServerDelay);
        }

        public static bool IsNeedUpdate()
        {
            return nextUpdateTime < (isUsingUTC ? DateTime.UtcNow : DateTime.Now);
        }

        public static TimeSpan LeftUpdateTime()
        {
            return (nextUpdateTime - (isUsingUTC ? DateTime.UtcNow : DateTime.Now));
        }

        public static void Update(double deltaTime)
        {
            Now = isUsingUTC ? DateTime.UtcNow : DateTime.Now;
            nextUpdateTime = Now.AddSeconds(ServerDelay);
            syncTime += deltaTime;
        }

        public static void SetSyncTime(double syncTime)
        {
            ServerTime.syncTime = syncTime;
        }

        public static bool IsNeedResync()
        {
            return syncTime > lastSyncRequestTime + resyncTime;
        }

        public static void SetTimeGap(double timeGap)
        {
            ServerTime.timeGap = timeGap;
        }

        public static double AdditionalTime(double updateTime)
        {
            if (timeGap == 0.0)
                return 0;

            double result;
            if (timeGap > 0.0)
            {
                double additional = Math.Min(timeGap, updateTime * additionalTimeRate);
                timeGap -= additional;
                result = -additional;
            }
            else
            {
                double additional = Math.Min(-timeGap, updateTime * additionalTimeRate);
                timeGap += additional;
                result = additional;
            }

            if (Math.Abs(timeGap) < 0.001)
                timeGap = 0.0;

            return result;
        }
    }
}