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

public enum SceneNameType
{
    Title_Scene=0,
    Login_Scene=1,
    Lobby_Scene=2,
    MultiGame_Scene=3,
    SoloGame_Scene=4
}
