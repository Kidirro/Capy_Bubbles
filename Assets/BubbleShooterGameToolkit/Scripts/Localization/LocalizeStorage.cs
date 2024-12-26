using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Localize", menuName = "")]
public class LocalizeStorage : ScriptableObject
{
    [SerializeField] private List<LocalizeTextObj> localizeTextObjs;
    [SerializeField] private List<LocalizeImageObj> localizeImageObjs;
    private static LocalizeStorage instanse;
    public void Init()
    {
        instanse ??= this;
    }
    public static string GetText(string key, Language language)
    {
        LocalizeTextObj text = instanse.localizeTextObjs.Find(x => x.key == key);
        switch (language)
        {
            case Language.English:
                return text.EnglishValue;
            case Language.Russia:
                return text.RussiaValue;
            case Language.Tajikistan:
                return text.TjsValue;
            default:
                UnityEngine.Debug.LogError("Not found Language");
                return "WARNING";
        }
    }
    public static Sprite GetImage(string key, Language language)
    {
        LocalizeImageObj text = instanse.localizeImageObjs.Find(x => x.key == key);
        switch (language)
        {
            case Language.English:
                return text.EnglishValue;
            case Language.Russia:
                return text.RussiaValue;
            case Language.Tajikistan:
                return text.TjsValue;
            default:
                UnityEngine.Debug.LogError("Not found Language");
                return null;
        }
    }
}

[Serializable]
class LocalizeTextObj
{
    public string key = "";
    public string RussiaValue = "Текст для перевода";
    public string EnglishValue = "Localize Text This";
    public string TjsValue = "!@$@#!#@";
}
[Serializable]
class LocalizeImageObj
{
    public string key = "";
    public Sprite RussiaValue;
    public Sprite EnglishValue;
    public Sprite TjsValue;
}