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
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;
using YG;

namespace BubbleShooterGameToolkit.Scripts.System
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager instance;
        public DateTime serverTime;
        public bool dateReceived;

        public delegate void DateReceived();

        public static event DateReceived OnDateReceived;

        [Header("Test date example: 2019-08-27 09:12:29")]
        public string TestDate = "";

        private float timePassed;
        public bool noConnection;
        public string lastDisableTime;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            GetServerTime();
        }

        private void OnEnable()
        {
            GetServerTime();
            lastDisableTime =
#if PLUGIN_YG_2
                YG2.saves.lastDisabledTime;
#else
                PlayerPrefs.GetString("LastDisabledTime", DateTime.Now.ToString("o"));
#endif
            StartCoroutine(SaveTimeProcess());
        }

        public float GetTimeSinceLastDisable()
        {
            var lastTime = DateTime.Parse(lastDisableTime, null, DateTimeStyles.RoundtripKind);
            return (float)(serverTime.ToLocalTime() - lastTime).TotalSeconds;
        }

        void GetServerTime()
        {
            StartCoroutine(getTime());
        }

        #if UNITY_ANDROID
        void OnApplicationFocus ( bool focus )
        {
            if (focus)
                GetServerTime();
            else
                dateReceived = false;
        }
        #endif

        #if UNITY_EDITOR || UNITY_IOS
        void OnApplicationPause(bool pause)
        {
            if (!pause)
                GetServerTime();
            else
                dateReceived = false;
        }
        #endif

        private void OnDisable()
        {
            dateReceived = false;
            // Save the current time as the last disabled time
            SaveTime();
            Debug.Log("On Disable " + PlayerPrefs.GetString("LastDisabledTime", DateTime.Now.ToString("o")));
        }

        private static void SaveTime()
        {
#if PLUGIN_YG_2
            YG2.saves.lastDisabledTime = DateTime.Now.ToString("o");
#else
            PlayerPrefs.SetString("LastDisabledTime", DateTime.Now.ToString("o"));
#endif
        }

        IEnumerator getTime()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                // No internet connection, use UTC system time
                serverTime = DateTime.UtcNow;
                noConnection = true;
            }
            else
            {
                noConnection = false;
                #if UNITY_WEBGL
                    serverTime = DateTime.UtcNow;
                #else
                UnityWebRequest www = UnityWebRequest.Get("https://worldtimeapi.org/api/timezone/Etc/UTC");
                yield return www.SendWebRequest();

                // Start a coroutine that will abort the request after a timeout
                StartCoroutine(AbortAfterSeconds(www, 5f));

                yield return new WaitUntil(() => www.downloadHandler.isDone || www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError);
                if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
                {
                    // Server error, use UTC system time
                    serverTime = DateTime.UtcNow;
                }
                else if (www.downloadHandler.text != "")
                {
                    TimeResponse timeResponse = JsonUtility.FromJson<TimeResponse>(www.downloadHandler.text);
                    serverTime = DateTime.ParseExact(timeResponse.datetime, "yyyy-MM-ddTHH:mm:ss.ffffffzzz", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                }
                else
                    serverTime = DateTime.UtcNow;

                if (TestDate != "" &&
                    (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor))
                    serverTime = DateTime.Parse(TestDate);
                #endif
                timePassed = 0;
                yield return null;
                dateReceived = true;
                OnDateReceived?.Invoke();
            }
        }

        IEnumerator AbortAfterSeconds(UnityWebRequest www, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (!www.isDone)
            {
                www.Abort();
            }
        }

        IEnumerator SaveTimeProcess()
        {
            var yieldWait = new WaitForSeconds(5);
            while (true)
            {
                yield return yieldWait;
                SaveTime();
            }
        }

        private void Update()
        {
            timePassed += Time.deltaTime;
            serverTime = serverTime.AddSeconds(Time.deltaTime);
        }
    }

    [Serializable]
    public class TimeResponse
    {
        public string datetime;
    }
}