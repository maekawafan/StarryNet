using System.Collections.Generic;

namespace StarryNet.StarryData
{
    public interface IStarryDataReference
    {
        bool Exist();
    }

    public interface IStarryDataReference<T> : IStarryDataReference
    {
        T Get();
    }

    public static class StarryDataReferenceEx
    {
        public static IEnumerable<T> Get<T>(this IStarryDataReference<T>[] array)
        {
            foreach (var value in array)
                yield return value.Get();
        }

        public static T[] GetArray<T>(this IStarryDataReference<T>[] array)
        {
            T[] result = new T[array.Length];
            for(int i = 0; i < array.Length; i++)
                result[i] = array[i].Get();
            return result;
        }
    }
}
