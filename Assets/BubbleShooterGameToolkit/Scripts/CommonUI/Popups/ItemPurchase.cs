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

using BubbleShooterGameToolkit.Scripts.Services;
using BubbleShooterGameToolkit.Scripts.Settings;
using BubbleShooterGameToolkit.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterGameToolkit.Scripts.CommonUI.Popups
{
    public class ItemPurchase : MonoBehaviour
    {
        public Button BuyItemButton;
        public TextMeshProUGUI price;
        public string _price;
        public TextMeshProUGUI count;
        public TextMeshProUGUI discountPercent;
        public string id;
        public ShopItemEditor settingsShopItem;

        private void OnEnable()
        {
            BuyItemButton.onClick.AddListener(BuyCoins);
#if PLUGIN_YG_2
            price.text = _price;
#else
            decimal priceValue = IAPManager.instance.GetProductLocalizedPrice(settingsShopItem.productID);
            if (priceValue > 0.01m)
            {
                price.text = IAPManager.instance.GetProductLocalizedPriceString(settingsShopItem.productID);
            }
#endif
        }

        private void BuyCoins()
        {
            
#if PLUGIN_YG_2
            GetComponentInParent<CoinsShop>().BuyCoins(settingsShopItem.productID);
#elif BEELINE
            BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<BubbleShooterGameToolkit.Scripts.CommonUI.Popups.BuyInfoPopup>().SetText(count.text, _price.ToString(), () =>
            GetComponentInParent<CoinsShop>().BuyCoins(id));

#else
            GetComponentInParent<CoinsShop>().BuyCoins(settingsShopItem.productID);

#endif
        }
    }

}