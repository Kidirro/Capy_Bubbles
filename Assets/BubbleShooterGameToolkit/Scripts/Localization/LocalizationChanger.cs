using System;
using System.Globalization;
using BubbleShooterGameToolkit.Scripts.Gameplay.GUI;
using UnityEngine;
using UnityEngine.UI;
using YG;
#if PLUGIN_YG_2
using YG; 
#endif

public class LocalizationChanger : MonoBehaviour
{
    internal static Language language { get; private set; } = Language.Russia;

    internal static Action<Language> languageActions;
    [SerializeField] LocalizeStorage localizeStorage;
    [SerializeField] CustomButton languageButton;
    [SerializeField] Image languageImage;
    [SerializeField] Sprite[] flags;


    void Awake()
    {
        var defaultLocalize = Application.systemLanguage;
        
#if PLUGIN_YG_2
        switch (YG2.lang)
        {
         case "ru":
             language = Language.Russia;
             break;
         case "tr":
             language = Language.Tajikistan;
             break;
         default:
             language = Language.English;
             break;
        }
#else
        language = defaultLocalize switch
        {
            SystemLanguage.Russian => Language.Russia,
            _ => Language.English
        };
#endif        

#if PLUGIN_YG_2
        if (YG2.saves.language != -1)
            language = (Language)YG2.saves.language;
#else
        if (PlayerPrefs.HasKey("Localization"))
            language = (Language)PlayerPrefs.GetInt("Localization");
#endif
        
        Debug.Log($"LocalizationChanger: Current OS language: {YG2.lang}. Save language: {language}");
        languageButton.onClick.AddListener(ChangeLanguage);
        localizeStorage.Init();
    }

    void Start()
    {
        languageImage.sprite = flags[(int)language];
        languageActions?.Invoke(language);
    }

    public void ChangeLanguage()
    {
        language = NextLanguage();
        languageImage.sprite = flags[(int)language];
        languageActions?.Invoke(language);
        
#if PLUGIN_YG_2
        YG2.saves.language = (int)language;
        YG2.SaveProgress();
#else
        PlayerPrefs.SetInt("Localization", (int)language);
#endif
    }

    private Language NextLanguage()
    {
        switch (language)
        {
            case Language.Russia:
                return Language.English;
            case Language.English:
                return Language.Tajikistan;
            case Language.Tajikistan:
                return Language.Russia;
            default:
                return Language.Russia;
        }
    }
}

public enum Language
{
    Russia = 0,
    English = 1,
    Tajikistan = 2
}