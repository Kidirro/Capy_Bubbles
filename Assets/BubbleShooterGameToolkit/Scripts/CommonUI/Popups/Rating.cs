using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

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
            var ratingData = await GetRating();
            CreateRating(ratingData);
            _loading.SetActive(false);
            _rating.SetActive(true);
        }

        private void CreateRating(List<RatingData> ratingDatas)
        {
            foreach (RectTransform child in _container)
            {
                Destroy(child.gameObject);
            }
            int i = 0;
            bool isPlayerInit=false;
            foreach (RatingData ratingData in ratingDatas)
            {
                i++;
                Instantiate(_prefab, _container).Initialize(i, ratingData.masked_phone, ratingData.counterLevel, ratingData.id == Model.id);
                if (ratingData.id == Model.id)
                {
                    isPlayerInit=true;
                    _player.Initialize(i, Model.phone, ratingData.counterLevel, true);
                }
            }
            if(!isPlayerInit)
            {
                 _player.Initialize(0, Model.phone, Model.playerData.counterLevel, true);
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
        [Serializable]
        public class RatingData
        {
            public int id { get; set; }
            public string masked_phone { get; set; }
            public int counterLevel { get; set; }
        }
    }
}