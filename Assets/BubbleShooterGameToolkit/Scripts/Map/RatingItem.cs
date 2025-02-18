using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RatingItem : MonoBehaviour
{
    [SerializeField] private TMP_Text place;
    [SerializeField] private TMP_Text nickname;
    [SerializeField] private TMP_Text counter;
    [SerializeField] private Image _mainBackground;
    [SerializeField] private Image _scoreBackground;
    private readonly Color32 mainColor = new Color32(0xb6, 0x6d, 0x42, 0xFF);
    private readonly Color32 scoreColor = new Color32(0x9d, 0x47, 0x2a, 0xFF);
    private readonly Color32 mainColorPlayer = new Color32(0x8f, 0x31, 0x1d, 0xFF);
    private readonly Color32 scoreColorPlayer = new Color32(0x69, 0x20, 0x13, 0xFF);
    public void Initialize(int place, string name, int counter, bool isPlayer)
    {
        this.place.text = place.ToString();
        if(place==0)
        this.place.text = "-";
        this.counter.text = counter.ToString();
        this.nickname.text = name;
        if (isPlayer)
        {
            _mainBackground.color = mainColorPlayer;
            _scoreBackground.color = scoreColorPlayer;
        }
        else
        {
            _mainBackground.color = mainColor;
            _scoreBackground.color = scoreColor;
        }
    }
}
