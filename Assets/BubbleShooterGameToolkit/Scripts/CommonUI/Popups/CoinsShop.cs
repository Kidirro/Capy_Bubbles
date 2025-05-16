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

using System.Linq;
using BubbleShooterGameToolkit.Scripts.Services;
using BubbleShooterGameToolkit.Scripts.Settings;
using BubbleShooterGameToolkit.Scripts.System;
using UnityEngine;

namespace BubbleShooterGameToolkit.Scripts.CommonUI.Popups
{
    public class CoinsShop : PopupWithCurrencyLabel
    {
        public GameObject awaitPanel;
        public ItemPurchase[] packs;
        protected ShopSettings shopSettings;
        private async void OnEnable()
        {
            shopSettings = Resources.Load<ShopSettings>("Settings/ShopSettings");
#if BEELINE
            Shop data = await Model.GetShopProduts();
            SetProducts(data);

#else
            for (int i = 0; i < packs.Length; i++)
            {
                packs[i].settingsShopItem = shopSettings.shopItems[i];
                packs[i].count.text = packs[i].settingsShopItem.coins.ToString();
                var discountPercent = packs[i].discountPercent;
                if (discountPercent != null)
                {
                    discountPercent.text = packs[i].settingsShopItem.discountPercent + "%";
                }
            }
#endif
            GameManager.instance.purchaseSucceded += PurchaseSucceded;
        }

        private void OnDisable()
        {
            GameManager.instance.purchaseSucceded -= PurchaseSucceded;
        }

        protected virtual void SetProducts(Shop data)
        {
            var prod = data.products.Where(x => x.gold > 0).ToArray();
            for (int i = 0; i < packs.Length; i++)
            {
                packs[i].settingsShopItem = shopSettings.shopItems.First(x => x.coins == prod[i].gold);
                packs[i].count.text = prod[i].gold.ToString();
                packs[i].id = prod[i].id;
                packs[i]._price = prod[i].price;
                packs[i].price.text = prod[i].price + "p.";
                var discountPercent = packs[i].discountPercent;
                if (discountPercent != null)
                {
                    discountPercent.text = packs[i].settingsShopItem.discountPercent + "%";
                }
            }
        }

        private void PurchaseSucceded(ShopItemEditor item)
        {
            var packButtonPos = packs.First(i => i.settingsShopItem == item).BuyItemButton.transform.position;
            if (item.coins > 0)
                topPanel.AnimateCoins(packButtonPos, "+" + item.coins, null);

            if (item.gems > 0)
                topPanel.AnimateGem(packButtonPos, "+" + item.gems, null);
        }

        public async void BuyCoins(string id)
        {
#if UNITY_WEBPLAYER
            GameManager.instance.PurchaseSucceded(id);
#elif MEGAFON
            awaitPanel.SetActive(true);
            var pack = packs.First(x => x.id == id);
            MegafonShopTJS.BuyProduct(pack,()=>PurchaseSucceded(pack.settingsShopItem));
            awaitPanel.SetActive(false);
#elif BEELINE
            awaitPanel.SetActive(true);
            bool isSucces = await Model.BuyProduct(id);
            if (isSucces)
                PurchaseSucceded(packs.First(x => x.id == id).settingsShopItem);
            awaitPanel.SetActive(false);
#else
            IAPManager.instance.BuyProduct(id);
#endif
        }
    }
}