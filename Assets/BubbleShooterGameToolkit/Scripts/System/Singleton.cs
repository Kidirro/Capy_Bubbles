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

namespace BubbleShooterGameToolkit.Scripts.System
{
    public class Singleton<T> where T : class
    {
        private static readonly object _lock = new object();
        private static T _instance;
        public static T instance
        {
            get
            {
                if (_instance != null) return _instance;

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)Activator.CreateInstance(typeof(T), true);
                    }
                }

                return _instance;
            }
        }

        protected Singleton()
        {
            Init();
        }

        public virtual void Init()
        {
            if(_instance == null)
                _instance = this as T;
        }
        
        public static void ResetInstance()
        {
            lock (_lock)
            {
                _instance = null;
            }
        }
    }
}