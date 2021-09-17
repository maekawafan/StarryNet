using System;
using System.Collections.Generic;

namespace StarryNet.StarryLibrary
{
    public static class CollectionEx
    {
        /// <summary>
        /// 컬렉션의 모든 아이템에 액션을 실행합니다.
        /// </summary>
        /// <param name="collection">대상 컬렉션</param>
        /// <param name="action">실행할 액션</param>
        static public void Action<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }

        /// <summary>
        /// 컬렉션이 비어있는지 여부를 리턴합니다.
        /// </summary>
        /// <param name="collection">비어있는지 여부를 검사할 컬렉션</param>
        static public bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        static public bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count == 0;
        }

        /// <summary>
        /// 컬렉션에 아이템을 추가합니다. 단, 같은 아이템이 있다면 추가하지 않고 false를 리턴합니다.
        /// </summary>
        /// <param name="collection">아이템을 추가할 컬렉션</param>
        /// <param name="item">추가할 아이템</param>
        static public bool AddOnly<T>(this ICollection<T> collection, T item)
        {
            if (collection.Contains(item))
                return false;
            collection.Add(item);
            return true;
        }

        /// <summary>
        /// 딕셔너리에 아이템을 추가합니다. 단, 같은 아이템이 있다면 추가하지 않고 false를 리턴합니다.
        /// </summary>
        /// <param name="dictionary">아이템을 추가할 딕셔너리</param>
        /// <param name="key">추가할 아이템의 키</param>
        /// <param name="value">추가할 아이템의 값</param>
        static public bool AddOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                return false;
            dictionary.Add(key, value);
            return true;
        }

        /// <summary>
        /// 컬렉션에 특정 조건을 만족하는 아이템이 몇 개인지 리턴합니다.
        /// </summary>
        /// <param name="collection">검사 대상 컬렉션</param>
        /// <param name="condition">특정 조건</param>
        /// <returns></returns>
        static public int ConditionCount<T>(this ICollection<T> collection, Predicate<T> condition)
        {
            int result = 0;
            foreach (var item in collection)
                if (condition(item))
                    result++;
            return result;
        }

        /// <summary>
        /// 반복자의 반복 횟수를 리턴합니다.
        /// </summary>
        /// <param name="enumerable">대상 반복자</param>
        /// <returns></returns>
        static public int Count<T>(this IEnumerable<T> enumerable)
        {
            int result = 0;
            foreach (var item in enumerable)
                result++;
            return result;
        }

        /// <summary>
        /// 반복자에 특정 조건을 만족하는 아이템이 몇 개인지 리턴합니다.
        /// </summary>
        /// <param name="enumerable">검사 대상 반복자</param>
        /// <param name="condition">특정 조건</param>
        /// <returns></returns>
        static public int ConditionCount<T>(this IEnumerable<T> enumerable, Predicate<T> condition)
        {
            int result = 0;
            foreach (var item in enumerable)
                if (condition(item))
                    result++;
            return result;
        }

        /// <summary>
        /// 컬렉션에 특정 조건을 만족하는 아이템을 리턴합니다.
        /// </summary>
        /// <param name="collection">검사 대상 컬렉션</param>
        /// <param name="condition">특정 조건</param>
        /// <returns></returns>
        static public T Find<T>(this ICollection<T> collection, Predicate<T> condition) where T : class
        {
            foreach (var item in collection)
                if (condition(item))
                    return item;
            return null;
        }

        /// <summary>
        /// 리스트의 랜덤한 값 하나를 리턴합니다.
        /// </summary>
        /// <param name="list">랜덤한 값을 가져올 리스트</param>
        static public T RandomPick<T>(this IList<T> list)
        {
            if (list.IsEmpty())
                return default(T);
            return list[Random.NextInt(list.Count - 1)];
        }

        /// <summary>
        /// 배열의 랜덤한 값 하나를 리턴합니다.
        /// </summary>
        /// <param name="array">랜덤한 값을 가져올 배열</param>
        static public T RandomPick<T>(this T[] array)
        {
            if (array.IsEmpty())
                return default(T);
            return array[Random.NextInt(array.Length - 1)];
        }
    }
}