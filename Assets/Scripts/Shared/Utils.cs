using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    public static Color GetColorFromString(string _color)
    {
        Color color;
        ColorUtility.TryParseHtmlString(_color, out color);
        return color;
    }
}
