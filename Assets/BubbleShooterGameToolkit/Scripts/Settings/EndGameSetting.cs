using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName ="Assect/123")]
public class EndGameSetting : ScriptableObject
{
    public EndgameMapObjectCostOLD[] mapObjectCost;
    public List<CostDataList> mapCost;

    // private void OnValidate()
    // {
    //     mapCost = new List<CostDataList>();
    //     for (int i = 0; i < mapObjectCost.Length; i++)
    //     {
    //         mapCost.Add(new CostDataList());
    //         for (int j = 0; j < mapObjectCost[i].rawMapObjectCost.Length; j++)
    //         {
    //             int cost = mapObjectCost[i].rawMapObjectCost[j];
    //             CurrencyType type = CurrencyType.Coin;
    //
    //             if (i == 0 || i == 1)
    //             {
    //                 if (cost <= 50)
    //                 {
    //                     cost = (cost / 30);
    //                     type = CurrencyType.Gem;
    //                 }
    //             }
    //             else if (i == 2)
    //             {
    //                 if (cost > 75)
    //                 {
    //                     cost = (cost / 30);
    //                     type = CurrencyType.Gem;
    //                 }
    //             }
    //             else
    //             {
    //                 cost = (cost / 30);
    //                 type = CurrencyType.Gem;
    //             }
    //
    //             mapCost[i].Data.Add(new CostData()
    //             {
    //                 Value = Mathf.Max(1, cost),
    //                 CurrencyType = type
    //             });
    //         }
    //     }
    // }
}

[Serializable]
public class EndgameMapObjectCostOLD
{
    [FormerlySerializedAs("mapObjectCost")] public int[] rawMapObjectCost;
    

}

[Serializable]
public class CostDataList
{
    public List<CostData> Data = new List<CostData>();
}

[Serializable]
public class CostData
{
    public CurrencyType CurrencyType; 
    public int Value;
}

public enum CurrencyType
{
    Coin,
    Gem
}