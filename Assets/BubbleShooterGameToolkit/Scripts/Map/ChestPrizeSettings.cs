using System;
using BubbleShooterGameToolkit.Scripts.Settings;
using UnityEngine;
[CreateAssetMenu(menuName = "123s/123")]
internal class ChestPrizeSettings:ScriptableObject
{
	public ManyPrizes[] rewardVisuals;
}
[Serializable]
public class ManyPrizes
{
	public RewardSettingSpin[] rewardVisuals;
}