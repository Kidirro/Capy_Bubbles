using UnityEngine.UI;

public class LocalizeImage : LocalizeObject
{
    [UnityEngine.SerializeField]
    private Image image;
   
    protected override void ChangeValue(Language value)
    {
        image.sprite = LocalizeStorage.GetImage(_localizeKey, value);
    }
}
