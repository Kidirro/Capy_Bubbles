using TMPro;

public class LocalizeText : LocalizeObject
{
    [UnityEngine.SerializeField]
    private TMP_Text text;
        protected override void ChangeValue(Language value)
    {
        text.text = LocalizeStorage.GetText(_localizeKey, value);
    }
}
