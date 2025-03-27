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

namespace BubbleShooterGameToolkit.Scripts.CommonUI.Popups
{
    public class GemsShop : CoinsShop
    {

        protected override void SetProducts(Shop data)
        {
            var prod = data.products.Where(x=> x.gems>0).ToArray();
            for (int i = 0; i < prod.Length; i++)
            {
                packs[i].settingsShopItem = shopSettings.shopItems.First(x=> x.gems==prod[i].gems);
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
            for(int i=prod.Length; i< packs.Length;i++){
                packs[i].gameObject.SetActive(false);
            }
        }
    }
}