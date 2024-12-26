using System;
using BubbleShooterGameToolkit.Scripts.Gameplay.GUI;
using UnityEngine;
using UnityEngine.UI;
using YG; 

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
#if UNITY_WEBGL
        language = (Language)YG2.saves.language;
#else

        if (PlayerPrefs.HasKey("Localization"))
            language = (Language)PlayerPrefs.GetInt("Localization");
#endif
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
        
#if UNITY_WEBGL
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