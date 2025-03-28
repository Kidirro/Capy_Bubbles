﻿// // ©2015 - 2024 Candy Smith
// // All rights reserved
// // Redistribution of this software is strictly not allowed.
// // Copy of this software can be obtained from unity asset store only.
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.

using BubbleShooterGameToolkit.Scripts.Audio;
using BubbleShooterGameToolkit.Scripts.Data;
using BubbleShooterGameToolkit.Scripts.Settings;
using UnityEngine;

namespace BubbleShooterGameToolkit.Scripts.CommonUI.Popups
{
	public class RewardPopup : PopupWithCurrencyLabel
	{
		public Transform iconPos;
		public Transform manyItem;
		private int _count;
		private ResourceObject _resource;
		private RewardSettingSpin rewardVisual;
		private RewardSettingSpin[] rewardVisuals;

		public override void ShowAnimationSound()
		{
			base.ShowAnimationSound();
			SoundBase.instance.PlaySound(SoundBase.instance.cheers);
		}

		public void SetReward(RewardSettingSpin rewardVisual)
		{
			this.rewardVisual = rewardVisual;
			var rewardObject = Instantiate(rewardVisual.rewardVisualPrefab, iconPos);
			rewardObject.transform.localPosition = Vector3.zero;
			rewardObject.transform.localRotation = Quaternion.identity;
			rewardObject.SetCount(rewardVisual.count);
			_count = rewardVisual.count;
			_resource = rewardVisual.resource;
		}
		public void SetReward(RewardSettingSpin[] rewardVisual)
		{
			rewardVisuals = rewardVisual;
			foreach (RewardSettingSpin reward in rewardVisual)
			{
				var rewardObject = Instantiate(reward.rewardVisualPrefab, manyItem);
				rewardObject.SetCount(reward.count);
			}
			_resource = rewardVisual[0].resource;
			_count = rewardVisual[0].count;
		}

		public override void Close()
		{
			rewardVisual?.resource.Add(rewardVisual.count);
			foreach (var rewardVisual in rewardVisuals)
			{
				rewardVisual.resource.Add(rewardVisual.count);
				if (_resource.name == "Coins")
					topPanel.AnimateCoins(iconPos.position, "+" + _count, () => base.Close());
				else if (_resource.name == "Life")
					topPanel.AnimateLife(iconPos.position, "", () => base.Close());
			}
			if (_resource.name == "Coins")
				topPanel.AnimateCoins(iconPos.position, "+" + _count, () => base.Close());
			else if (_resource.name == "Life")
				topPanel.AnimateLife(iconPos.position, "", () => base.Close());
			else
				base.Close();
		}


	}
}
