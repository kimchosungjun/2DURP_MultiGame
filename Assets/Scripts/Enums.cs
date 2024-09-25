using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enums : MonoBehaviour
{
    public static int GetEnumValue<T>(T enumType) where T : Enum
    {
        return Convert.ToInt32(enumType);
    }

    public static string GetEnumName<T>(T enumType) where T : Enum
    {
        return enumType.ToString();
    }
}

namespace OmokStoneEnum
{
    public enum StoneColor
    {
        White,
        Black
    }
}

public enum ResourceType
{
    Stone,
    UI,
    Image
}
