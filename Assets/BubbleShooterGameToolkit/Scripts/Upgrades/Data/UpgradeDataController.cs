using System;
using System.Collections.Generic;
using UnityEngine;

public static class UpgradeDataController
{
    private static string _resourcePath = "Upgrade/UpgradeData";
    
    private static UpgradeDataDictionary _upgradeData;
    
    private static Dictionary<string, UpgradeData> _upgradeDataContainer = new Dictionary<string, UpgradeData>();
    
    public static event Action<UpgradeData> OnUpgradeDataChanged = delegate { }; 

    public static Dictionary<string, UpgradeData> GetDataContainer()
    {
        if (_upgradeDataContainer.Count == 0)
        {
            if (_upgradeData == null)
            {
                _upgradeData = Resources.Load<UpgradeDataDictionary>(_resourcePath);
            }
            
            foreach (var upgradeData in _upgradeData.RawUpgradeData)
            {
                _upgradeDataContainer.Add(upgradeData.UpgradeId, upgradeData);
            }
        }
        
        return _upgradeDataContainer;
    }

    public static int GetUpgradePrice(UpgradeData upgradeData)
    {
        int level = GetUpgradeLevel(upgradeData);
        return upgradeData.UpgradeCostBase + level * upgradeData.UpgradeCostPerLevel;
    }

    public static int GetUpgradeLevel(UpgradeData upgradeData)
    {
        int level = 0;

        //TODO Сделать получение уровня
        return Model.playerData.upgrdadeData.TryGetValue(upgradeData.UpgradeId, out level) ? level : 0;
    }

    public static int GetUpgradeLevel(string upgradeData)
    {
        return GetUpgradeLevel(GetDataContainer()[upgradeData]);
    }

    public static void SetUpgradeLevel(UpgradeData upgradeData, int level)
    {
        int curlevel = 0;
        curlevel = level;
        
        Model.playerData.upgrdadeData[upgradeData.UpgradeId] = curlevel;

        Model.SetSave();
    }

    public static void LevelUpUpgrade(UpgradeData upgradeData)
    {
        if (IsMaxLevel(upgradeData)) return;
        int currentLevel = GetUpgradeLevel(upgradeData);
        SetUpgradeLevel(upgradeData, currentLevel + 1);
        OnUpgradeDataChanged?.Invoke(upgradeData);
    }

    public static bool IsMaxLevel(UpgradeData upgradeData)
    {
        return GetUpgradeLevel(upgradeData) >= upgradeData.MaxLevel;
    }
    
}
