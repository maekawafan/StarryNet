using System;
using System.Collections.Generic;

namespace StarryNet.StarryDataGenerator
{
    public static partial class ClassGenerator
    {
        public const string tabSpace = "    ";
        public const char splitCharacter = ',';

        public static Dictionary<string, ClassData> classDictionary = new Dictionary<string, ClassData>();

        public enum DataType
        {
            None,
            String,
            Bool,
            Byte,
            Sbyte,
            Short,
            Ushort,
            Int,
            Uint,
            Long,
            Ulong,
            Float,
            Double,
            Decimal,
            DateTime,
            BigInteger,
            LocalEnum,
            GlobalEnum,
            Vector,
            Color,
            Reference,
        }

        public static Dictionary<string, (DataType type, bool isStartWord)> typeTable = new Dictionary<string, (DataType, bool)>()
        {
            { "string",     (DataType.String        , false) },
            { "bool",       (DataType.Bool          , false) },
            { "byte",       (DataType.Byte          , false) },
            { "sbyte",      (DataType.Sbyte         , false) },
            { "short",      (DataType.Short         , false) },
            { "ushort",     (DataType.Ushort        , false) },
            { "int",        (DataType.Int           , false) },
            { "uint",       (DataType.Uint          , false) },
            { "long",       (DataType.Long          , false) },
            { "ulong",      (DataType.Ulong         , false) },
            { "float",      (DataType.Float         , false) },
            { "double",     (DataType.Double        , false) },
            { "decimal",    (DataType.Decimal       , false) },
            { "datetime",   (DataType.DateTime      , false) },
            { "biginteger", (DataType.BigInteger    , false) },
            { "enum",       (DataType.LocalEnum     , false) },
            { "enum:",      (DataType.GlobalEnum    , true)  },
            { "vector",     (DataType.Vector        , false) },
            { "color",      (DataType.Color         , false) },
        };

        public static Dictionary<DataType, string> codeTypeTable = new Dictionary<DataType, string>()
        {
            {DataType.String,       "string"},
            {DataType.Bool,         "bool"},
            {DataType.Byte,         "byte"},
            {DataType.Sbyte,        "sbyte"},
            {DataType.Short,        "short"},
            {DataType.Ushort,       "ushort"},
            {DataType.Int,          "int"},
            {DataType.Uint,         "uint"},
            {DataType.Long,         "long"},
            {DataType.Ulong,        "ulong"},
            {DataType.Float,        "float"},
            {DataType.Double,       "double"},
            {DataType.Decimal,      "decimal"},
            {DataType.DateTime,     "DateTime"},
            {DataType.BigInteger,   "BigInteger"},
            {DataType.Color,        "Color"},
        };

        public static Dictionary<string, DataType> autoParseTypeTable = new Dictionary<string, DataType>()
        {
            { "id", DataType.Uint },
            { "name", DataType.String },
        };
    }
}