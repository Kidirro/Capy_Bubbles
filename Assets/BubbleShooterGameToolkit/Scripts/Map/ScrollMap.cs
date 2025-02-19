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
using UnityEngine;
using UnityEngine.UI;

namespace BubbleShooterGameToolkit.Scripts.Map
{
    public class ScrollMap : MonoBehaviour
    {

        [SerializeField]
        private ScrollRect scrollRect;
        
        private float aspectRatioConst = 1920/1440f; 
        private const float mapLenght = 2700;
        void Start()
        {
        }

        private void OnEnable()
        {
            MapManager.OnLastLevelPosition += ScrollToAvatar;
        }

        private void OnDisable()
        {
            MapManager.OnLastLevelPosition -= ScrollToAvatar;
        }

        private void ScrollToAvatar(Vector2 vector2)
        {

            StartCoroutine(ScrollDelay(vector2));

        }

        private IEnumerator ScrollDelay(Vector2 vector2)
        {
            Vector2 avatarPositionInLocalSpace = scrollRect.transform.InverseTransformPoint(vector2);

            scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, Math.Max(avatarPositionInLocalSpace.y + mapLenght, 3500));
            yield return null;
            avatarPositionInLocalSpace = scrollRect.transform.InverseTransformPoint(vector2);
            Vector2 contentPositionInLocalSpace = scrollRect.transform.InverseTransformPoint(scrollRect.content.position);
            Vector2 contentAnchoredPosition = contentPositionInLocalSpace - avatarPositionInLocalSpace;

            float aspectRatio =
#if PLUGIN_YG_2
                aspectRatioConst;
#else
                Screen.height / Screen.width;
#endif

            float centerOffset = aspectRatio * 500f;
            scrollRect.content.anchoredPosition = new Vector2(0, contentAnchoredPosition.y + centerOffset);
        }
    }
}