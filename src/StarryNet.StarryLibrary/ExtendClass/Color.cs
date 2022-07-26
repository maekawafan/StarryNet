using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class ColorEX
{
    public static Color SetAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public static string ToHex(this Color color)
    {
        return $"#{ColorUtility.ToHtmlStringRGBA(color)}";
    }
}
