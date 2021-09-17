using System;

namespace StarryNet.StarryLibrary
{
    public static class ArrayEx
    {
        public static T[] Reverse<T>(this T[] array)
        {
            int length = array.Length;
            for (int near = 0; near < length / 2; near++)
            {
                int far = length - 1 - near;
                T temp = array[near];
                array[near] = array[far];
                array[far] = temp;
            }
            return array;
        }

        /// <summary>
        /// <see cref="ArraySegment"/>의 값을 배열로 복사해 리턴합니다.
        /// </summary>
        public static T[] MakeArray<T>(this ArraySegment<T> arraySegment)
        {
            T[] result = new T[arraySegment.Count];
            Buffer.BlockCopy(arraySegment.Array, arraySegment.Offset, result, 0, arraySegment.Count);
            return result;
        }

    }
}