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
using BubbleShooterGameToolkit.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if PLUGIN_YG_2
using YG;
#endif

namespace BubbleShooterGameToolkit.Scripts.CommonUI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        
        [SerializeField] private TextMeshProUGUI versionText;
        
        private void Start()
        {
            playButton.onClick.AddListener(StartGame);
     
            versionText.text = "";       
#if PLUGIN_YG_2
                YG2.GameReadyAPI();
                YG2.ConsumePurchases();
                
                versionText.text = $"v{Application.version}";
#endif
        }

        private void StartGame()
        {
            SceneLoader.instance.GoToMap();
        }
    }
}