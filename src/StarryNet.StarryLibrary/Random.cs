using StarryLibrary;
using System;

namespace StarryNet.StarryLibrary
{
    public class Random
    {
        const int W = 32;
        const int R = 32;
        const int M1 = 3;
        const int M2 = 24;
        const int M3 = 10;
        const double fact = 2.32830643653869628906e-10;

        static private uint globalStateIndex = 0;
        static private uint[] globalState = new uint[R];
        static private uint globalZ0;
        static private uint globalZ1;
        static private uint globalZ2;

        private uint stateIndex = 0;
        private uint[] state = new uint[R];
        private uint z0;
        private uint z1;
        private uint z2;

        static private uint MatPos(int t, uint v) => v ^ (v >> t);
        static private uint MatNeg(int t, uint v) => v ^ (v << (-t));

        static Random()
        {
            System.Random rand = new System.Random(DateTime.Now.Millisecond);
            for (int j = 0; j < R; j++)
                globalState[j] = (uint)(rand.NextDouble() * uint.MaxValue) << 16 | (uint)(rand.NextDouble() * uint.MaxValue);
        }

        public Random()
        {
            System.Random rand = new System.Random(DateTime.Now.Millisecond);
            for (int j = 0; j < R; j++)
                state[j] = (uint)(rand.NextDouble() * uint.MaxValue) << 16 | (uint)(rand.NextDouble() * uint.MaxValue);
        }

        public Random(int seed)
        {
            System.Random rand = new System.Random(seed);
            for (int j = 0; j < R; j++)
                state[j] = (uint)(rand.NextDouble() * uint.MaxValue) << 16 | (uint)(rand.NextDouble() * uint.MaxValue);
        }

        private static double Next()
        {
            globalZ0 = globalState[(globalStateIndex + 31) & 0x0000001fU];
            globalZ1 = (globalState[globalStateIndex]) ^ MatPos(8, globalState[(globalStateIndex + 3) & 0x0000001fU]);
            globalZ2 = MatNeg(-19, globalState[(globalStateIndex + 24) & 0x0000001fU]) ^ MatNeg(-14, globalState[(globalStateIndex + 10) & 0x0000001fU]);

            globalState[globalStateIndex] = globalZ1 ^ globalZ2;
            globalState[(globalStateIndex + 31) & 0x0000001fU] = MatNeg(-11, globalZ0) ^ MatNeg(-7, globalZ1) ^ MatNeg(-13, globalZ2);
            globalStateIndex = (globalStateIndex + 31) & 0x0000001fU;
            return globalState[globalStateIndex] * fact;
        }

        // 10만번 중 146회 Clamp됨
        public static double NextGaussian()
        {
            return ((Math.PI + Math.Sqrt(-2.0 * Math.Log(Next())) * Math.Sin(2.0 * Math.PI * Next())) / (Math.PI * 2.0)).Clamp(0.0, 1.0);
        }

        private double Get()
        {
            z0 = state[(stateIndex + 31) & 0x0000001fU];
            z1 = (state[stateIndex]) ^ MatPos(8, state[(stateIndex + 3) & 0x0000001fU]);
            z2 = MatNeg(-19, state[(stateIndex + 24) & 0x0000001fU]) ^ MatNeg(-14, state[(stateIndex + 10) & 0x0000001fU]);

            state[stateIndex] = z1 ^ z2;
            state[(stateIndex + 31) & 0x0000001fU] = MatNeg(-11, z0) ^ MatNeg(-7, z1) ^ MatNeg(-13, z2);
            stateIndex = (stateIndex + 31) & 0x0000001fU;
            return state[stateIndex] * fact;
        }

        private double GetGaussian()
        {
            return ((Math.PI + Math.Sqrt(-2.0 * Math.Log(Get())) * Math.Sin(2.0 * Math.PI * Get())) / (Math.PI * 2.0)).Clamp(0.0, 1.0);
        }

        public static float NextFloat(float max)                        => (float)(Next() * max);
        public static float NextFloat(float min, float max)             => (float)(Next() * (max - min)) + min;
        public static double NextDouble(double max)                     => Next() * max;
        public static double NextDouble(double min, double max)         => (Next() * (max - min)) + min;
        public static float NextFloatGaussian(float max)                => (float)(NextGaussian() * max);
        public static float NextFloatGaussian(float min, float max)     => (float)(NextGaussian() * (max - min)) + min;
        public static double NextDoubleGaussian(double max)             => NextGaussian() * max;
        public static double NextDoubleGaussian(double min, double max) => (NextGaussian() * (max - min)) + min;
        public static int NextInt(int max)                              => (int)(Next() * (max + 1));
        public static int NextInt(int min, int max)                     => (int)(Next() * (max - min + 1)) + min;
        public static uint NextUint(uint max)                           => (uint)(Next() * (max + 1U));
        public static uint NextUint(uint min, uint max)                 => (uint)(Next() * (max - min + 1U)) + min;
        public static long NextLong(long max)                           => (long)(Next() * (max + 1L));
        public static long NextLong(long min, long max)                 => (long)(Next() * (max - min + 1L)) + min;
        public static ulong NextUlong(ulong max)                        => (ulong)(Next() * (max + 1L));
        public static ulong NextUlong(ulong min, ulong max)             => (ulong)(Next() * (max - min + 1L)) + min;

        public float GetFloat(float max)                                => (float)(Get() * max);
        public float GetFloat(float min, float max)                     => (float)(Get() * (max - min)) + min;
        public double GetDouble(double max)                             => Get() * max;
        public double GetDouble(double min, double max)                 => (Get() * (max - min)) + min;
        public float GetFloatGaussian(float max)                        => (float)(GetGaussian() * max);
        public float GetFloatGaussian(float min, float max)             => (float)(GetGaussian() * (max - min)) + min;
        public double GetDoubleGaussian(double max)                     => GetGaussian() * max;
        public double GetDoubleGaussian(double min, double max)         => (GetGaussian() * (max - min)) + min;
        public int GetInt(int max)                                      => (int)(Get() * (max + 1));
        public int GetInt(int min, int max)                             => (int)(Get() * (max - min + 1)) + min;
        public uint GetUint(uint max)                                   => (uint)(Get() * (max + 1U));
        public uint GetUint(uint min, uint max)                         => (uint)(Get() * (max - min + 1U)) + min;
        public long GetLong(long max)                                   => (long)(Get() * (max + 1L));
        public long GetLong(long min, long max)                         => (long)(Get() * (max - min + 1L)) + min;
        public ulong GetUlong(ulong max)                                => (ulong)(Get() * (max + 1L));
        public ulong GetUlong(ulong min, ulong max)                     => (ulong)(Get() * (max - min + 1L)) + min;
    }
}