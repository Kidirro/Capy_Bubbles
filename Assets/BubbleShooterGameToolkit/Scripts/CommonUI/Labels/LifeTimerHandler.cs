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

using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using BubbleShooterGameToolkit.Scripts.System;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterGameToolkit.Scripts.CommonUI.Labels
{
    public class LifeTimerHandler : MonoBehaviour
    {
        public Button buyLifeButton;
        private int gameSettingsMaxLife;

        private void OnEnable()
        {
            buyLifeButton?.onClick.AddListener(BuyLife);
            gameSettingsMaxLife = GameManager.instance.GameSettings.MaxLife;
            GameManager.instance.life.OnResourceUpdate += UpdateButton;
            UpdateButton(GameManager.instance.life.GetResource());
        }
        
        private void OnDisable()
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.life.OnResourceUpdate -= UpdateButton;
            }
        }

        private void UpdateButton(int count)
        {
            if (buyLifeButton != null)
            {
                buyLifeButton.interactable = !GameManager.instance.life.IsEnough(gameSettingsMaxLife);
            }
        }

        public void BuyLife()
        {
            if(GameManager.instance.life.IsEnough(gameSettingsMaxLife))
                return;
            MenuManager.instance.ShowPopup<LifeShop>();
        }
    }
}