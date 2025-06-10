using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.CommonUI;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using BubbleShooterGameToolkit.Scripts.System;
using TMPro;
using UnityEngine;

public class EventClaimPrizesWindow : PopupWithCurrencyLabel
{
    [SerializeField]
    private TextMeshProUGUI placeText;
    
    [SerializeField]
    private TextMeshProUGUI scoreText;
    
    
    [SerializeField]
    private PrizeCard prizeCardPrefab;


    private SpecialEventManager.EventReward _reward;
    
    [SerializeField]
    private TopPanel topPanelStatic;

    protected override void Awake()
    {
    }

    public void ShowReward(SpecialEventManager.EventReward reward)
    {
        _reward = reward;
        placeText.text = _reward.place.ToString() + " место";
        
        prizeCardPrefab.SetPrize(new SpecialEventManager.Prize(){place = 0, rewards = _reward.rewards});
    }

    public void ClaimReward()
    {
        //ShowCoinsSpendFX(topPanel.transform.position);

        if (_reward.rewards.gold > 0)
        {
            topPanelStatic.AnimateCoins(scoreText.transform.position, "+" + _reward.rewards.gold, () => { });
            GameManager.instance.coins.Add(_reward.rewards.gold);
        }

        if (_reward.rewards.gem > 0)
        {
            topPanelStatic.AnimateGem(scoreText.transform.position, "+" + _reward.rewards.gem, () => { });
            GameManager.instance.gem.Add(_reward.rewards.gem);
        }

        SpecialEventManager.SetClaimPrize(_reward.reward_id);
        gameObject.SetActive(false);
    }
  
}
