using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BubbleShooterGameToolkit.Scripts.Gameplay.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class SpecialEventManager
{
    public static EventData ChosenEventData = null; 
    public static List<EventData> CurrentEventDataList = new ();
        
    public static async Task<List<EventData>> GetCurrentEventData()
    {
        var request = UnityWebRequest.Get(Model.backend + "/event/current");
        request.SetRequestHeader("Content-Type", ""); // Очищаем
        await request.SendWebRequest();
        switch (request.responseCode)
        {
            case 200:
                Debug.Log("Event: " + request.downloadHandler.text);
                var tempEventDataList = JsonHelper.FromJson<EventDataRaw>(request.downloadHandler.text);
                CurrentEventDataList = new List<EventData>();
                foreach (var tempEventData in tempEventDataList)
                {
                    CurrentEventDataList.Add(new EventData()
                    {
                        id = tempEventData.id,
                        name = tempEventData.name,
                        event_type = tempEventData.event_type,
                        start_date = DateTime.Parse(tempEventData.start_date, null, System.Globalization.DateTimeStyles.RoundtripKind),
                        end_date = DateTime.Parse(tempEventData.end_date, null, System.Globalization.DateTimeStyles.RoundtripKind),
                        logo = tempEventData.logo,
                        level_id = tempEventData.level_id,
                        prizes = tempEventData.prizes
                        
                    });
                    
                    
                }
                break;
            case 500:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                CurrentEventDataList = new();
                break;

            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                CurrentEventDataList = new();
                break;
        }
        return CurrentEventDataList;
    }
    //501 - ошибка при отправке. Ивент окончен 
    
    public static async Task<UpdateResultResponse> PostResultEvent(int eventId, float result)
    {
        var request = UnityWebRequest.Post(Model.backend + $"event/result?event_id={eventId}", result.ToString(CultureInfo.InvariantCulture), "application/json");
        request.SetRequestHeader("accessToken", Model.GetToken());
        await request.SendWebRequest();
        var resultResponse =new UpdateResultResponse();
        switch (request.responseCode)
        {
            case 200:
                Debug.Log("Event: " + request.downloadHandler.text);
                resultResponse = JsonUtility.FromJson<UpdateResultResponse>(request.downloadHandler.text);
                break;
            case 500:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                break;
        
            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.text);
                break;
        }
        return resultResponse;
    }
    
    public static async Task<LeaderboardResponse> GetLeaderBoard(int eventId, float limit)
    {
        var request = UnityWebRequest.Get(Model.backend + $"event/leaderboard?event_id={eventId}&limit={limit}");
        request.SetRequestHeader("accessToken", Model.GetToken());
        await request.SendWebRequest();
        var result = new LeaderboardResponse();
        switch (request.responseCode)
        {
            case 200:
                Debug.Log("Event: " + request.downloadHandler.text);
                result = JsonUtility.FromJson<LeaderboardResponse>(request.downloadHandler.text);
                break;
            default:
                BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<BubbleShooterGameToolkit.Scripts.CommonUI.Popups.InfoPopup>().SetCustomText($"Ошибка при загрузке рейтинга ({request.responseCode}).\n Попробуйте позже.");
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.text);
                return null;
        }
        return result;
    }

    public static async Task<List<EventReward>> GetUnclaimedPrizes()
    {
        var request = UnityWebRequest.Get(Model.backend + $"prizes/unclaimed");
        request.SetRequestHeader("accessToken", Model.GetToken());
        await request.SendWebRequest();
        List<EventReward> result = new();
        switch (request.responseCode)
        {
            case 200:
                Debug.Log("Prizes: " + request.downloadHandler.text);
                result = JsonHelper.FromJson<EventReward>(request.downloadHandler.text).ToList();
                break;
            case 500:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                //CurrentEventData = new EventData();
                break;
        
            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.text);
                //CurrentEventData = null;
                break;
        }
        return result;
    }
    
    public static async void SetClaimPrize(int id)
    {
        var request = UnityWebRequest.Post(Model.backend + $"prizes/claim", id.ToString(), "application/json");
        request.SetRequestHeader("accessToken", Model.GetToken());
        await request.SendWebRequest();
        switch (request.responseCode)
        {
            case 200:
                Debug.Log("Event: " + request.downloadHandler.text);
                //CurrentEventData = JsonUtility.FromJson<EventData>(request.downloadHandler.text);
                break;
            case 500:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                //CurrentEventData = new EventData();
                break;
        
            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.text);
                //CurrentEventData = null;
                break;
        }
    }

    public static void SetChosenEventData(EventData eventData)
    {
        ChosenEventData = eventData;
    }
    
    public class EventData
    {
        public int id;
        public string name;
        public EventType event_type; // Или использовать кастомный enum
        public DateTime start_date;
        public DateTime end_date;
        public int logo;
        public List<int> level_id;
        public List<Prize> prizes;
    }
    
    [Serializable]
    public class EventDataRaw
    {
        public int id;
        public string name;
        public EventType event_type; // Или использовать кастомный enum
        public string start_date;
        public string end_date;
        public int logo;
        public List<int> level_id;
        public List<Prize> prizes;
    }
    
    [Serializable]
    public class UpdateResultResponse
    {
        public bool updated;
        public float result;
        public int place;
    }

    [Serializable]
    public class Prize
    {
        public int place;
        public Reward rewards;
    }

    [Serializable]
    public class Reward
    {
        public int gold;
        public int gem;
    }
    
    [Serializable]
    public class EventReward
    {
        public int reward_id;
        public int event_id;
        public int place;
        public Reward rewards;
    }
    
    public enum EventType
    {
        time,
        score
    }
    
    [Serializable]
    public class LeaderboardResponse
    {
        public List<LeaderboardEntry> top;
        public LeaderboardEntry current_user;
    }

    [Serializable]
    public class LeaderboardEntry
    {
        public int user_id;
        public float result;
        public string user_name; // может быть null у current_user
        public int place; // может отсутствовать у current_user
        public Reward rewards; // может быть null
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{\"array\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}