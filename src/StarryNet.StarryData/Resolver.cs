using MessagePack;
using MessagePack.Formatters;

using System;
using System.Collections.Generic;
using StarryNet.StarryLibrary;

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
        { typeof(ItemInstance), new ItemInstanceFormatter() },
        { typeof(ItemInstance[]), new ArrayFormatter<ItemInstance>() },
        { typeof(List<ItemInstance>), new ListFormatter<ItemInstance>() },
        { typeof(CharacterData), new CharacterDataFormatter() },
        { typeof(CharacterData[]), new ArrayFormatter<CharacterData>() },
        { typeof(List<CharacterData>), new ListFormatter<CharacterData>() },
        { typeof(CharacterInstance), new CharacterInstanceFormatter() },
        { typeof(CharacterInstance[]), new ArrayFormatter<CharacterInstance>() },
        { typeof(List<CharacterInstance>), new ListFormatter<CharacterInstance>() },
        { typeof(SkillData), new SkillDataFormatter() },
        { typeof(SkillData[]), new ArrayFormatter<SkillData>() },
        { typeof(List<SkillData>), new ListFormatter<SkillData>() },
        { typeof(SkillInstance), new SkillInstanceFormatter() },
        { typeof(SkillInstance[]), new ArrayFormatter<SkillInstance>() },
        { typeof(List<SkillInstance>), new ListFormatter<SkillInstance>() },
        { typeof(WeaponData), new WeaponDataFormatter() },
        { typeof(WeaponData[]), new ArrayFormatter<WeaponData>() },
        { typeof(List<WeaponData>), new ListFormatter<WeaponData>() },
        { typeof(WeaponInstance), new WeaponInstanceFormatter() },
        { typeof(WeaponInstance[]), new ArrayFormatter<WeaponInstance>() },
        { typeof(List<WeaponInstance>), new ListFormatter<WeaponInstance>() },
        { typeof(BulletData), new BulletDataFormatter() },
        { typeof(BulletData[]), new ArrayFormatter<BulletData>() },
        { typeof(List<BulletData>), new ListFormatter<BulletData>() },
        { typeof(BulletInstance), new BulletInstanceFormatter() },
        { typeof(BulletInstance[]), new ArrayFormatter<BulletInstance>() },
        { typeof(List<BulletInstance>), new ListFormatter<BulletInstance>() },
        { typeof(WallData), new WallDataFormatter() },
        { typeof(WallData[]), new ArrayFormatter<WallData>() },
        { typeof(List<WallData>), new ListFormatter<WallData>() },
        { typeof(WallInstance), new WallInstanceFormatter() },
        { typeof(WallInstance[]), new ArrayFormatter<WallInstance>() },
        { typeof(List<WallInstance>), new ListFormatter<WallInstance>() },
        { typeof(FloorData), new FloorDataFormatter() },
        { typeof(FloorData[]), new ArrayFormatter<FloorData>() },
        { typeof(List<FloorData>), new ListFormatter<FloorData>() },
        { typeof(FloorInstance), new FloorInstanceFormatter() },
        { typeof(FloorInstance[]), new ArrayFormatter<FloorInstance>() },
        { typeof(List<FloorInstance>), new ListFormatter<FloorInstance>() },
        { typeof(BuffData), new BuffDataFormatter() },
        { typeof(BuffData[]), new ArrayFormatter<BuffData>() },
        { typeof(List<BuffData>), new ListFormatter<BuffData>() },
        { typeof(BuffInstance), new BuffInstanceFormatter() },
        { typeof(BuffInstance[]), new ArrayFormatter<BuffInstance>() },
        { typeof(List<BuffInstance>), new ListFormatter<BuffInstance>() },
        { typeof(MapData), new MapDataFormatter() },
        { typeof(MapData[]), new ArrayFormatter<MapData>() },
        { typeof(List<MapData>), new ListFormatter<MapData>() },
        { typeof(MapInstance), new MapInstanceFormatter() },
        { typeof(MapInstance[]), new ArrayFormatter<MapInstance>() },
        { typeof(List<MapInstance>), new ListFormatter<MapInstance>() },
        { typeof(UserInfoData), new UserInfoDataFormatter() },
        { typeof(UserInfoData[]), new ArrayFormatter<UserInfoData>() },
        { typeof(List<UserInfoData>), new ListFormatter<UserInfoData>() },
        { typeof(UserInfoInstance), new UserInfoInstanceFormatter() },
        { typeof(UserInfoInstance[]), new ArrayFormatter<UserInfoInstance>() },
        { typeof(List<UserInfoInstance>), new ListFormatter<UserInfoInstance>() },
        { typeof(ServerInfoData), new ServerInfoDataFormatter() },
        { typeof(ServerInfoData[]), new ArrayFormatter<ServerInfoData>() },
        { typeof(List<ServerInfoData>), new ListFormatter<ServerInfoData>() },
        { typeof(ServerInfoInstance), new ServerInfoInstanceFormatter() },
        { typeof(ServerInfoInstance[]), new ArrayFormatter<ServerInfoInstance>() },
        { typeof(List<ServerInfoInstance>), new ListFormatter<ServerInfoInstance>() },
        { typeof(ReviveTextData), new ReviveTextDataFormatter() },
        { typeof(ReviveTextData[]), new ArrayFormatter<ReviveTextData>() },
        { typeof(List<ReviveTextData>), new ListFormatter<ReviveTextData>() },
        { typeof(ReviveTextInstance), new ReviveTextInstanceFormatter() },
        { typeof(ReviveTextInstance[]), new ArrayFormatter<ReviveTextInstance>() },
        { typeof(List<ReviveTextInstance>), new ListFormatter<ReviveTextInstance>() },
        { typeof(LoadingTextData), new LoadingTextDataFormatter() },
        { typeof(LoadingTextData[]), new ArrayFormatter<LoadingTextData>() },
        { typeof(List<LoadingTextData>), new ListFormatter<LoadingTextData>() },
        { typeof(LoadingTextInstance), new LoadingTextInstanceFormatter() },
        { typeof(LoadingTextInstance[]), new ArrayFormatter<LoadingTextInstance>() },
        { typeof(List<LoadingTextInstance>), new ListFormatter<LoadingTextInstance>() },
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