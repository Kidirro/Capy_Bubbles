using UnityEngine;

public abstract class LocalizeObject : MonoBehaviour
{
    [SerializeField] protected string _localizeKey;
    void Start()
    {
        ChangeValue(LocalizationChanger.language);
    }
    void OnEnable()
    {
        LocalizationChanger.languageActions += ChangeValue;
    }
    void OnDisable()
    {
        LocalizationChanger.languageActions -= ChangeValue;
    }

    protected abstract void ChangeValue(Language value);
}
