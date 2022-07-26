using StarryNet.StarryLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorEX
{
    public static Vector2 AddX(this Vector2 value, float x)
    {
        return new Vector2(value.x + x, value.y);
    }

    public static Vector2 AddY(this Vector2 value, float y)
    {
        return new Vector2(value.x, value.y + y);
    }

    public static Vector2 SetX(this Vector2 value, float x)
    {
        return new Vector2(x, value.y);
    }

    public static Vector2 SetY(this Vector2 value, float y)
    {
        return new Vector2(value.x, y);
    }

    public static Vector3 AddX(this Vector3 value, float x)
    {
        return new Vector3(value.x + x, value.y, value.z);
    }

    public static Vector3 AddY(this Vector3 value, float y)
    {
        return new Vector3(value.x, value.y + y, value.z);
    }

    public static Vector3 AddZ(this Vector3 value, float z)
    {
        return new Vector3(value.x, value.y, value.z + z);
    }

    public static Vector3 SetX(this Vector3 value, float x)
    {
        return new Vector3(x, value.y, value.z);
    }

    public static Vector3 SetY(this Vector3 value, float y)
    {
        return new Vector3(value.x, y, value.z);
    }

    public static Vector3 SetZ(this Vector3 value, float z)
    {
        return new Vector3(value.x, value.y, z);
    }

    public static Vector2 RadianToVector2(this float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(this float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

    public static Vector3 RadianToVector3(this float radian)
    {
        return new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0.0f);
    }

    public static Vector3 DegreeToVector3(this float degree)
    {
        return RadianToVector3(degree * Mathf.Deg2Rad);
    }

    public static float ToRadian(this Vector2 vector)
    {
        Vector2 normal = vector.normalized;
        return Mathf.Atan2(vector.y, vector.x);
    }

    public static float ToDegree(this Vector2 vector)
    {
        return vector.ToRadian() * Mathf.Rad2Deg;
    }

    public static DirectionFlag GetDirection(this Vector2 value)
    {
        DirectionFlag result = DirectionFlag.None;

        if (0.0f > value.x)
            result |= DirectionFlag.Left;
        else if (0.0f < value.x)
            result |= DirectionFlag.Right;
        if (0.0f > value.y)
            result |= DirectionFlag.Down;
        else if (0.0f < value.y)
            result |= DirectionFlag.Up;

        return result;
    }

    public static Vector2 ToVector(this DirectionFlag value)
    {
        Vector2 result = Vector2.zero;

        if (DirectionFlag.Down == (DirectionFlag.Down & value))
            result += Vector2.down;
        else if (DirectionFlag.Up == (DirectionFlag.Up & value))
            result += Vector2.up;
        if (DirectionFlag.Left == (DirectionFlag.Left & value))
            result += Vector2.left;
        else if (DirectionFlag.Right == (DirectionFlag.Right & value))
            result += Vector2.right;

        return result.normalized;
    }

    public static Vector2 GetLeftAngle(this Vector2 value)
    {
        return new Vector2(value.y, -value.x);
    }

    public static Vector2 GetRightAngle(this Vector2 value)
    {
        return new Vector2(-value.y, value.x);
    }

    public static Vector2 Rotate(this Vector2 value, float radian)
    {
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);
        return new Vector2(value.x * cos - value.y * sin, value.x * sin + value.y * cos);
    }

    public static Vector2 RotateDegree(this Vector2 value, float degree)
    {
        return Rotate(value, degree * Mathf.Deg2Rad);
    }

    public static Vector2 ToVector2(this Vector3 value)
    {
        return new Vector2(value.x, value.y);
    }

    public static Vector2 ToVector2(this Vector4 value)
    {
        return new Vector2(value.x, value.y);
    }

    public static Vector3 ToVector3(this Vector2 value)
    {
        return new Vector3(value.x, value.y, 0.0f);
    }

    public static Vector3 ToVector3(this Vector4 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }

    public static Vector2 ToVector2(this Vector2Int value)
    {
        return new Vector2(value.x, value.y);
    }

    public static Vector3 ToVector3(this Vector3Int value)
    {
        return new Vector3(value.x, value.y, value.z);
    }

    public static Vector2Int ToVector2Int(this Vector3Int value)
    {
        return new Vector2Int(value.x, value.y);
    }

    public static Vector3Int ToVector3Int(this Vector2Int value)
    {
        return new Vector3Int(value.x, value.y, 0);
    }

    public static Vector2Int ToVector2IntFloor(this Vector2 value)
    {
        return new Vector2Int(Mathf.FloorToInt(value.x), Mathf.FloorToInt(value.y));
    }

    public static Vector2Int ToVector2IntRound(this Vector2 value)
    {
        return new Vector2Int(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));
    }

    public static Vector2Int ToVector2IntCeil(this Vector2 value)
    {
        return new Vector2Int(Mathf.CeilToInt(value.x), Mathf.CeilToInt(value.y));
    }

    public static float Cross(this Vector2 a, Vector2 b)
    {
        return (a.x * b.y) - (a.y * b.x);
    }

    public static bool Close(this Vector2 value, Vector2 other, float distance)
    {
        return (value - other).sqrMagnitude < distance * distance;
    }

    public static bool Close(this Vector3 value, Vector3 other, float distance)
    {
        return (value - other).sqrMagnitude < distance * distance;
    }
}
