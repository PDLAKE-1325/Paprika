using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    public int screenIndex = 0;
    public bool UI_on_off = true;
    public string my_displayName;
    public List<string> designSetting;
    public List<StringIntPair> itemPrices;
}
