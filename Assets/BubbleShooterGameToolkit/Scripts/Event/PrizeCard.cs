using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrizeCard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI placeText;
    
    [SerializeField]
    private List<GameObject> placeIcons = new List<GameObject>();

    [Space, SerializeField] private GameObject coinsHandler;
    [SerializeField] private TextMeshProUGUI coinsValueText;
    
    [Space, SerializeField] private GameObject heartsHandler;
    [SerializeField] private TextMeshProUGUI heartsValueText;
    
    [Space, SerializeField] private GameObject gemsHandler;
    [SerializeField] private TextMeshProUGUI gemsValueText;
    
    private SpecialEventManager.Prize _prize;

    public void SetPrize(SpecialEventManager.Prize prize)
    {
        _prize = prize;
        UpdateUIRatingPlayerCard();
    }

    public void UpdateUIRatingPlayerCard()
    {
        if (placeText != null)
        {
            placeText.text = -_prize.place == null ? "-" : _prize.place.ToString();
        }

        for (int i = 0; i < placeIcons.Count; i++)
        {
            placeIcons[i].SetActive(i == _prize.place + 1);
        }

        coinsHandler.SetActive(_prize.rewards.gold>0);
        coinsValueText.text = _prize.rewards.gold.ToString();
        
        gemsHandler.SetActive(_prize.rewards.gem>0);
        gemsValueText.text = _prize.rewards.gem.ToString();
    }
}
