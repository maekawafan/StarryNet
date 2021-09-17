using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace StarryNet.StarryLibrary
{
    public static class ClassEx
    {
        /// <summary>
        /// 특정 클래스의 자식 클래스를 리턴합니다.
        /// </summary>
        public static Type GetSubClass<T>(Predicate<Type> condition)
        {
            foreach (Type type in typeof(T).Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(T)) && condition(type))
                    return type;
            }
            return null;
        }

        /// <summary>
        /// 특정 클래스의 자식 클래스를 리턴합니다.
        /// </summary>
        public static Type GetSubClass(this Type type, Predicate<Type> condition)
        {
            foreach (Type currentType in type.Assembly.GetTypes())
            {
                if (currentType.IsSubclassOf(type) && condition(currentType))
                    return currentType;
            }
            return null;
        }

        /// <summary>
        /// 특정 클래스의 자식 클래스 리스트를 리턴합니다.
        /// </summary>
        public static List<Type> GetSubClassList<T>()
        {
            return new List<Type>(typeof(T).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(T))));
        }

        /// <summary>
        /// 특정 어셈블리에서 클래스의 자식 클래스 리스트를 리턴합니다.
        /// </summary>
        public static List<Type> GetSubClassList<T>(Assembly assembly)
        {
            return new List<Type>(assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(T))));
        }

        /// <summary>
        /// 특정 클래스의 자식 클래스 리스트를 리턴합니다.
        /// </summary>
        public static List<Type> GetSubClassList(this Type type)
        {
            return new List<Type>(type.Assembly.GetTypes().Where(t => t.IsSubclassOf(type)));
        }

        /// <summary>
        /// 특정 클래스의 자식 클래스를 다른 어셈블리를 포함하여 검색해 리스트를 리턴합니다.
        /// </summary>
        public static List<Type> GetSubClassList(this Type type, params Assembly[] assemblies)
        {
            List<Type> result = new List<Type>();
            result.AddRange(type.Assembly.GetTypes().Where(t => t.IsSubclassOf(type)));
            foreach (var assembly in assemblies)
                result.AddRange(assembly.GetTypes().Where(t => t.IsSubclassOf(type)));
            return result;
        }

        /// <summary>
        /// 특정 타입의 리스트를 만들어 리턴합니다.
        /// </summary>
        public static IList CreateList(this Type type)
        {
            Type genericType = typeof(List<>).MakeGenericType(type);
            return Activator.CreateInstance(genericType) as IList;
        }
    }
}
