using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI upgradeNameText;
    
    [SerializeField]
    private TextMeshProUGUI upgradeDescriptionText;
    
    [SerializeField]
    private Image upgradeIcon;
    
    [SerializeField]
    private TextMeshProUGUI upgradeCostText;
    
    [SerializeField]
    private TextMeshProUGUI upgradeLevelText;

    private UpgradeData _currentUpgrade;
    
    public void SetUpgradeData(UpgradeData upgradeData)
    {
        _currentUpgrade = upgradeData;
        
        if (upgradeNameText != null)
        {
            //upgradeNameText.text = LocalizeStorage.GetText(upgradeData.UpgradeName, LocalizationChanger.language);
            upgradeNameText.text = upgradeData.UpgradeName;
        }

        if (upgradeDescriptionText != null)
        {
            //upgradeDescriptionText.text = LocalizeStorage.GetText(upgradeData.UpgradeDescription, LocalizationChanger.language);
            upgradeDescriptionText.text = upgradeData.UpgradeDescription;
        }

        if (upgradeIcon != null)
        {
            upgradeIcon.sprite = upgradeData.UpgradeIcon;
        }

        if (upgradeCostText != null)
        {
            upgradeCostText.text = UpgradeDataController.IsMaxLevel(upgradeData)? 
                "MAX" : 
                (UpgradeDataController.GetUpgradePrice(upgradeData)) + (upgradeData.UpgradeCostType == UpgradeData.CostType.Coin? "COIN":"GEM");
        }

        if (upgradeLevelText != null)
        {
            upgradeLevelText.text = UpgradeDataController.GetUpgradeLevel(upgradeData).ToString() + "/" +
                                    upgradeData.MaxLevel;
        }
    }

    public void TryUpgrade()
    {
        if (UpgradeDataController.IsMaxLevel(_currentUpgrade)) return;

        switch (_currentUpgrade.UpgradeCostType)
        {
            case UpgradeData.CostType.Gem:
                if (GameManager.instance.gem.Consume(UpgradeDataController.GetUpgradePrice(_currentUpgrade)))
                {
                    UpgradeDataController.LevelUpUpgrade(_currentUpgrade);
                    Model.SetSave();
                }
                break;
            case UpgradeData.CostType.Coin:
                if (GameManager.instance.coins.Consume(UpgradeDataController.GetUpgradePrice(_currentUpgrade)))
                {
                    UpgradeDataController.LevelUpUpgrade(_currentUpgrade);
                    Model.SetSave();
                }
                break;
        }
    }
}
