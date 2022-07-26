using System;
using System.Collections.Generic;
using System.Text;

namespace StarryLibrary
{
    public static class StarryMath
    {
        public static int Diff(int a, int b)
        {
            return Math.Max(a, b) - Math.Min(a, b);
        }

        public static float Diff(float a, float b)
        {
            return Math.Max(a, b) - Math.Min(a, b);
        }

        public static double Diff(double a, double b)
        {
            return Math.Max(a, b) - Math.Min(a, b);
        }

        /// <summary>
        /// 정수를 나눕니다. 나머지가 있을 경우, 올림 처리합니다.
        /// </summary>
        public static int CeilingDivide(this int operand, int value)
        {
            return operand % value == 0 ? operand / value : operand / value + 1;
        }

        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            return value.CompareTo(min) < 0 ? min : value.CompareTo(max) > 0 ? max : value;
        }
    }
}
