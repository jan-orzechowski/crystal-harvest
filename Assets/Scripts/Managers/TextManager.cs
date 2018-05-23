using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextManager
{
    Dictionary<string, string> strings;

    public TextManager() 
    {
        strings = StaticData.LoadStrings();
    }

    public string GetText(string key)
    {
        if (strings.ContainsKey(key)) return strings[key];
        else return (key + " - NO TEXT FOUND");
    }
}
