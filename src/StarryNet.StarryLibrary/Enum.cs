using System;


namespace StarryNet.StarryLibrary
{
    public enum ServerType
    {
        LobbyServer,
        GameArbiter,
        GameServer,
        OperationTool,
    }

    [Flags]
    public enum DirectionFlag
    {
        None    = 0,
        Down    = 0b0001,
        Right   = 0b0010,
        Up      = 0b0100,
        Left    = 0b1000,
        All     = 0b1111,
    }

    public enum Status
    {
        AllLock,
        MoveLock,
        FireLock,
        ReloadLock,
        SkillLock,
        ConstructLock,
        WeaponChangeLock,
    }
}