using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "UpgradeData", menuName = "UpgradeData")]
public class UpgradeDataDictionary : ScriptableObject
{
    public List<UpgradeData> RawUpgradeData;


}
