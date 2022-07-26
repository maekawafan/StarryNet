using System;
using System.Collections.Generic;
using System.Text;
using StarryNet.StarryLibrary;

namespace StarryNet.StarryData
{
    public static partial class BuffEx
    {
        public static bool IsTimeOver(this BuffInstance buff)
        {
            return buff.endTime < ServerTime.SyncFloatTime;
        }

        public static BuffInstance CreateBuff(this BuffData buffData, float time)
        {
            BuffInstance instance = new BuffInstance(buffData);
            instance.startTime = ServerTime.SyncFloatTime;
            instance.endTime = ServerTime.SyncFloatTime + time;
            return instance;
        }

        public static BuffInstance CreateBuff(this BuffData buffData, float time, float value)
        {
            BuffInstance instance = new BuffInstance(buffData);
            instance.startTime = ServerTime.SyncFloatTime;
            instance.endTime = ServerTime.SyncFloatTime + time;
            instance.value = value;
            return instance;
        }

        public static float TotalLifeTime(this BuffInstance buff)
        {
            return buff.endTime - buff.startTime;
        }

        public static float LifeTime(this BuffInstance buff)
        {
            return buff.endTime - ServerTime.SyncFloatTime;
        }
    }
}
