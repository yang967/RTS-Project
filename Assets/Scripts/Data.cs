using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    public static int playerNumber
    {
        get => PlayerPrefs.GetInt("playerNumber", 2);
        set => PlayerPrefs.SetInt("playerNumber", value);
    }

    public static int EffectNumber
    {
        get => PlayerPrefs.GetInt("effectNumber", 2);
    }
}
