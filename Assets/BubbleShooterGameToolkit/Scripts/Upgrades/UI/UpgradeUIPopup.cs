using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UpgradeUIPopup : MonoBehaviour
{
    [SerializeField]
    private List<UpgradeViewer> upgradeViewers = new List<UpgradeViewer>();

    private async void Start()
    {
        await UniTask.Delay(1000);
        
        var upgradeDatas = UpgradeDataController.GetDataContainer().Values.ToList();
        
        for (int i = 0; i < upgradeViewers.Count; i++)
        {
            if (upgradeDatas.Count <= i)
            {
                upgradeViewers[i].gameObject.SetActive(false);
            }
            else
            {
                upgradeViewers[i].gameObject.SetActive(true);
                upgradeViewers[i].SetUpgradeData(upgradeDatas[i]);
            }
        }
    }
}
