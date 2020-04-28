using System.Collections;
using System.Collections.Generic;
using System;

public class TextManager
{
    public static Language currentLanguage = Language.English;

    Dictionary<string, string> strings;

    public TextManager() 
    {
        strings = StaticLanguageData.Load(currentLanguage);
    }

    public void ChangeLanguage(Language newLanguage)
    {
        currentLanguage = newLanguage;

        strings = StaticLanguageData.Load(newLanguage);    
    }

    public string GetText(string key)
    {
        if (strings.ContainsKey(key)) return strings[key];
        else return (key + " - NO TEXT FOUND");
    }
}
