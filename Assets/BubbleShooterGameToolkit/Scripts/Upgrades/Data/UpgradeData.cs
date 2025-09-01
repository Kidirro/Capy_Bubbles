using System;
using UnityEngine;

[Serializable]
public class UpgradeData
{
    public string UpgradeId;
    
    public string UpgradeName;
    public string UpgradeDescription;
    public Sprite UpgradeIcon;
    
    [Space]
    public CostType UpgradeCostType;
    public int UpgradeCostBase;
    public int UpgradeCostPerLevel;
    
    [Space]
    public int MaxLevel;

    public enum CostType
    {
        Coin,
        Gem,
    }
}
