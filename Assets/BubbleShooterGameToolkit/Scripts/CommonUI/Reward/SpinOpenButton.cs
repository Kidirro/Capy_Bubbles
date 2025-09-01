// // ©2015 - 2024 Candy Smith
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

using System;
using System.Collections;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterGameToolkit.Scripts.CommonUI.Reward
{
    public class SpinOpenButton : MonoBehaviour
    {
        [SerializeField]
        private Button spinButton;

        [SerializeField]
        private GameObject freeSpinLabel;

        private UpgradeData _upgradeFreeSpin =>
            UpgradeDataController.GetDataContainer()["FreeDailySpin"];
        
        private void OnEnable()
        {
            spinButton.onClick.AddListener(OpenSpin);
            CheckFree();
        }

        public bool IsEnableByUpgrade()
        {
            if (UpgradeDataController.GetUpgradeLevel(_upgradeFreeSpin) <= 0) return false;
            DateTime lastTime = DateTime.Parse(PlayerPrefs.GetString("LastSpin", DateTime.MinValue.ToString()));

            return (DateTime.Now - lastTime).TotalDays >= 1;
        } 
        
        private void CheckFree()
        {
            if (Model.playerData == null)
            {
                StartCoroutine(WaitPlayerData());
                return;
            }  
            bool freeSpin = PlayerPrefs.GetInt("FreeSpin", 0) == 0 || IsEnableByUpgrade();
            freeSpinLabel.SetActive(freeSpin);
        }

        private void OpenSpin()
        {
            MenuManager.instance.ShowPopup<LuckySpin>(null, (x) => CheckFree());
        }

        private IEnumerator WaitPlayerData()
        {
            while (Model.playerData == null)
            {
                yield return null;
            }
            
            CheckFree();
        }
    }
}