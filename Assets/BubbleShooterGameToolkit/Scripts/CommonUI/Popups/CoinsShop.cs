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
using System.Linq;
using BubbleShooterGameToolkit.Scripts.Services;
using BubbleShooterGameToolkit.Scripts.Settings;
using BubbleShooterGameToolkit.Scripts.System;
using UnityEngine;
using YG;

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
            
            Shop data = await Model.GetShopProduts();
            FilterData(data);     
            GameManager.instance.purchaseSucceded += PurchaseSucceded;
        }

        private void OnDisable()
        {
            var manager = GameManager.instance;
            if (manager != null) manager.purchaseSucceded -= PurchaseSucceded;
        }

        protected virtual void FilterData(Shop data)
        {
            var prod = data.products.Where(x => x.gold > 0).ToArray();
            SetProducts(prod);
        } 
        
        protected void SetProducts(ProductShop[] data)
        {
            var prod = data;
            for (int i = 0; i < packs.Length; i++)
            {
                if (i >= prod.Length)
                {
                    packs[i].gameObject.SetActive(false);
                    continue;
                }
                packs[i].gameObject.SetActive(true);
                var yg_purchase = YG2.PurchaseByID(prod[i].id);
                
                //packs[i].settingsShopItem = ;
                if (prod[i].gems > 0)
                {
                    packs[i].count.text = prod[i].gems.ToString();
                }
                else
                {
                    packs[i].count.text = prod[i].gold.ToString();
                }
                packs[i].id = prod[i].id;
                packs[i]._price = yg_purchase.price;
                packs[i].price.text = IAPManager.instance.GetProductLocalizedPriceString(prod[i].id);
                var discountPercent = packs[i].discountPercent;
                if (discountPercent != null)
                {
                    discountPercent.text = packs[i].settingsShopItem.discountPercent + "%";
                }
            }
        }

        private void PurchaseSucceded(ShopItemEditor item)
        {
            var packButtonPos = packs.First(i => i.id == item.productID).BuyItemButton.transform.position;
            
            if (item.coins > 0)
                topPanel.AnimateCoins(packButtonPos, "+" + item.coins, null);

            if (item.gems > 0)
                topPanel.AnimateGem(packButtonPos, "+" + item.gems, null);
        }
        public async void BuyCoins(string id)
        {
#if PLUGIN_YG_2 || UNITY_ANDROID || UNITY_IOS
            IAPManager.instance.BuyProduct(id);
#elif BEELINE
            awaitPanel.SetActive(true);
            await Model.BuyProduct(id);
            PurchaseSucceded(Convert.ToInt32(id));
            awaitPanel.SetActive(false);
#endif
        }
    }
}