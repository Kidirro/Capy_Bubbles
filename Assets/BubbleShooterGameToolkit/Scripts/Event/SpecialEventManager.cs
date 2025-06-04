using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BubbleShooterGameToolkit.Scripts.Gameplay.Managers;
using UnityEngine;
using UnityEngine.Networking;

public class SpecialEventManager
{
    public static EventData ChosenEventData = null; 
    public static List<EventData> CurrentEventDataList;
        
    public static async Task<List<EventData>> GetCurrentEventData()
    {
        var request = UnityWebRequest.Get(Model.backend + "/event/current");
        await request.SendWebRequest();
        switch (request.responseCode)
        {
            case 200:
                Debug.Log("Event: " + request.downloadHandler.text);
                var tempEventDataList = JsonHelper.FromJson<EventDataRaw>(request.downloadHandler.text);
                foreach (var tempEventData in tempEventDataList)
                {
                    CurrentEventDataList.Add(new EventData()
                    {
                        id = tempEventData.id,
                        name = tempEventData.name,
                        event_type = tempEventData.event_type,
                        start_date = DateTime.Parse(tempEventData.start_date),
                        end_date = DateTime.Parse(tempEventData.end_date),
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
    
    public static async void PostResultEvent(int eventId, float result)
    {
        var request = UnityWebRequest.Post(Model.backend + $"event/result?event_id={eventId}", result.ToString(), "application/json");
        request.SetRequestHeader("accessToken", Model.GetToken());
        await request.SendWebRequest();
        switch (request.responseCode)
        {
            case 200:
                Debug.Log("Event: " + request.downloadHandler.text);
                break;
            case 500:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                break;
        
            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.text);
                break;
        }
    }
    
    public static async Task<EventData> GetLeaderBoard(int eventId, float limit)
    {
        var request = UnityWebRequest.Get(Model.backend + $"event/leaderboard?event_id={eventId}&limit={limit}");
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
        return new ();
    }

    public static async Task<EventData> GetUnclaimedPrizes()
    {
        var request = UnityWebRequest.Get(Model.backend + $"prizes/unclaimed");
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
        return new ();
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
        public string logo;
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
        public string logo;
        public List<int> level_id;
        public List<Prize> prizes;
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
    public enum EventType
    {
        score
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