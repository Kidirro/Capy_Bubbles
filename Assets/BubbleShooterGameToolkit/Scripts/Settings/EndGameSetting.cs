using System;
using UnityEngine;

[CreateAssetMenu(menuName ="Assect/123")]
public class EndGameSetting : ScriptableObject
{
    public EndgameMapObjectCost[] mapObjectCost;
}
[Serializable]
public class EndgameMapObjectCost
{
    public int[] mapObjectCost;

}
