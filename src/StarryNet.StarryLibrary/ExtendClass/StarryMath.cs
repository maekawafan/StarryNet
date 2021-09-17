using System;
using System.Collections.Generic;
using System.Text;

namespace StarryNet.StarryLibrary
{
    public static class StarryMath
    {
        /// <summary>
        /// 정수를 나눕니다. 나머지가 있을 경우, 올림 처리합니다.
        /// </summary>
        public static int CeilingDivide(this int operand, int value)
        {
            return operand % value == 0 ? operand / value : operand / value + 1;
        }
    }
}
