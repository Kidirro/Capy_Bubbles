using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using YG;
using YG.Utils.LB;

namespace BubbleShooterGameToolkit.Scripts.CommonUI.Popups
{
    public class Rating : Popup
    {
        [SerializeField] private GameObject _loading;
        [SerializeField] private GameObject _rating;
        [SerializeField] private RatingItem _prefab;
        [SerializeField] private Transform _container;
        [SerializeField] private RatingItem _player;
        
        async void OnEnable()
        {
            _loading.SetActive(true);
            _rating.SetActive(false);
#if PLUGIN_YG_2
            YG2.onGetLeaderboard -= GetLBData;
            YG2.onGetLeaderboard += GetLBData;
            
            YG2.GetLeaderboard("LevelCountLeaderboard",5,5);
#else
            var ratingData = await GetRating();
            CreateRating(ratingData);
            _loading.SetActive(false);
            _rating.SetActive(true);
#endif
        }

#if PLUGIN_YG_2
        protected override void OnDisable()
        {
#if PLUGIN_YG_2
            YG2.onGetLeaderboard -= GetLBData;
#endif
            base.OnDisable();
        }
#endif

        private void CreateRating(List<RatingData> ratingDatas)
        {
            foreach (RectTransform child in _container)
            {
                Destroy(child.gameObject);
            }
            int i = 0;
            bool isPlayerInit=false;
            bool isPlayerAuth = YG2.player.auth;
            
            foreach (RatingData ratingData in ratingDatas)
            {
                bool isCurrentPlayer = 
#if PLUGIN_YG_2
                    isPlayerAuth && ratingData.id_YG == YG2.player.id
#else
                    ratingData.id == Model.id
#endif
                    ;
                i++;
                Instantiate(_prefab, _container).Initialize(
#if PLUGIN_YG_2                    
                    ratingData.rank_YG,
                    ratingData.nickname_YG,
#else 
                    i,
                    ratingData.masked_phone,
#endif
                    ratingData.counterLevel,isCurrentPlayer);
                if (isCurrentPlayer)
                {
                    isPlayerInit=true;
                    _player.Initialize(
#if PLUGIN_YG_2           
                        ratingData.rank_YG,
                        isPlayerAuth ? YG2.player.name : LocalizeStorage.GetText("You",LocalizationChanger.language),
#else                    
                        i,
                        Model.phone,
#endif
                        ratingData.counterLevel, true);
                }
            }
            if(!isPlayerInit)
            {
                _player.Initialize(0,
#if PLUGIN_YG_2
                    isPlayerAuth ? YG2.player.name : LocalizeStorage.GetText("You",LocalizationChanger.language),
                    Model.playerData.score,
#else
                     Model.phone, 
                     Model.playerData.counterLevel,
#endif
                    true);
            }
        }

        private static async Task<List<RatingData>> GetRating()
        {
            var request = UnityWebRequest.Get(Model.backend + "user/rating");
            var process = request.SendWebRequest();
            while (!process.isDone)
            {
                await Task.Yield();
            }
            switch (request.responseCode)
            {
                case 200:
                    Debug.Log(request.downloadHandler.text);
                    return JsonConvert.DeserializeObject<List<RatingData>>(request.downloadHandler.text);//.Replace(":\"", ":").Replace("\",", ",").Replace("\"}", "}"));


                default:
                    Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                    return new List<RatingData>();

            }
        }
        
#if PLUGIN_YG_2
        private void GetLBData(LBData lbData)
        {
            List<RatingData> ratingDatas = new List<RatingData>();
            foreach (var data in lbData.players)
            {
                ratingDatas.Add(new RatingData()
                {
                    id_YG = data.uniqueID,
                    counterLevel =data.score,
                    rank_YG = data.rank,
                    nickname_YG = data.name
                } );
            }
            
            
            CreateRating(ratingDatas);
            _loading.SetActive(false);
            _rating.SetActive(true);
        }
#endif        
        
        [Serializable]
        public class RatingData
        {
            public int id { get; set; }
            public string masked_phone { get; set; }
            public int counterLevel { get; set; }
            
#if PLUGIN_YG_2            
            public int rank_YG { get; set; }
            public string nickname_YG { get; set; }
            
            public string id_YG { get; set; }
#endif
        }
    }
}