using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GemPurchaseButton : MonoBehaviour
{
    [SerializeField]
    private Button buyItemButton;
    
    [SerializeField]
    private int priceValue;
    
    [SerializeField]
    private TextMeshProUGUI priceText;
    
    [SerializeField]
    private int goldValue;
    
    [SerializeField]
    private TextMeshProUGUI goldText;

    private void OnEnable()
    {
        buyItemButton.onClick.AddListener(BuyCoins);
        
        priceText.text = priceValue.ToString();
        goldText.text = goldValue.ToString();
    }

    private void BuyCoins()
    {
        if (GameManager.instance.gem.GetResource() < priceValue)
        {
            return;
        }
        GameManager.instance.coins.Add(goldValue);
        GameManager.instance.gem.Add(-priceValue);
    }
}
