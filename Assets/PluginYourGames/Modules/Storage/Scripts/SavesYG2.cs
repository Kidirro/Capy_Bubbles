
using System;

namespace YG
{
    [System.Serializable]
    public partial class SavesYG
    {
        public int idSave;
        
        public int language = -1;
        public int sound;
        public int music;

        public string levelData = String.Empty;
        public int laslLevel = 1; 
        
        public string playerDataJson = String.Empty;

    }
}
