
using System;

namespace YG
{
    [System.Serializable]
    public partial class SavesYG
    {
        public int idSave;
        
        public int language = -1;
        public int sound = 1;
        public int music = 1;
        
        public bool isFreeSpin = true;

        public string levelData = String.Empty;
        public int laslLevel = 1; 
        
        public string playerDataJson = String.Empty;

        public string dailyBonusDay = "";
        public int rewardStreak = -1;
        public string lastDisabledTime = DateTime.Now.ToString("o");

        public int scoreLevelLeaderboard = 0;
        public int scoreObjectLeaderboard = 0;
    }
}
