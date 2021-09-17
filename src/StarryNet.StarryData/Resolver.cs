using MessagePack;
using MessagePack.Formatters;

using System;
using System.Collections.Generic;

namespace StarryNet.StarryData
{
    public class DataResolver : IFormatterResolver
    {
        public static readonly DataResolver Instance = new DataResolver();

        private DataResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                Formatter = (IMessagePackFormatter<T>)DataResolveryResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class DataResolveryResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, object> FormatterMap = new Dictionary<Type, object>()
    {
        { typeof(ItemData), new ItemDataFormatter() },
        { typeof(ItemData[]), new ArrayFormatter<ItemData>() },
        { typeof(List<ItemData>), new ListFormatter<ItemData>() },
    };

        internal static object GetFormatter(Type t)
        {
            object formatter;
            if (FormatterMap.TryGetValue(t, out formatter))
            {
                return formatter;
            }

            return null;
        }
    }
    
}