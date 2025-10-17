using System;

namespace YG
{
    public partial class SavesYG
    {
        public int intExample;
        public bool[] boolExample = new[] { false, true, false };
        public string strExample;
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

        public int chestLevels = 0;
        public bool rewardClaimed = false;
    }
}