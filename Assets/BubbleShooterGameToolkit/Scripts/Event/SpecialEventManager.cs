using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using BubbleShooterGameToolkit.Scripts.Gameplay.Managers;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using YG;
using YG.Utils.LB;
using Random = UnityEngine.Random;

public class SpecialEventManager
{
    public static EventData ChosenEventData = null; 
    public static List<EventData> CurrentEventDataList = new ();
    public static DateTime StartDay = new DateTime(2025,10,23,0,0,0);
    
    const string  STATIC_EVENT_DATA = "[{\"id\":14,\"name\":\"Мандариновый ивент\",\"event_type\":\"time\",\"start_date\":\"2025-10-14T13:57:00+00:00\",\"end_date\":\"2025-10-21T11:00:00+00:00\",\"logo\":\"1\",\"level_id\":[18,38,108,58,88],\"prizes\":[{\"place\":1,\"rewards\":{\"gold\":500,\"gem\":10}},{\"place\":2,\"rewards\":{\"gold\":500,\"gem\":8}},{\"place\":3,\"rewards\":{\"gold\":500,\"gem\":5}},{\"place\":4,\"rewards\":{\"gold\":500,\"gem\":3}},{\"place\":5,\"rewards\":{\"gold\":500,\"gem\":2}},{\"place\":6,\"rewards\":{\"gold\":500,\"gem\":1}},{\"place\":7,\"rewards\":{\"gold\":400,\"gem\":1}},{\"place\":8,\"rewards\":{\"gold\":300,\"gem\":1}},{\"place\":9,\"rewards\":{\"gold\":200,\"gem\":1}},{\"place\":10,\"rewards\":{\"gold\":100,\"gem\":1}},{\"place\":11,\"rewards\":{\"gold\":300}},{\"place\":12,\"rewards\":{\"gold\":300}},{\"place\":13,\"rewards\":{\"gold\":300}},{\"place\":14,\"rewards\":{\"gold\":300}},{\"place\":15,\"rewards\":{\"gold\":300}},{\"place\":16,\"rewards\":{\"gold\":200}},{\"place\":17,\"rewards\":{\"gold\":200}},{\"place\":18,\"rewards\":{\"gold\":200}},{\"place\":19,\"rewards\":{\"gold\":200}},{\"place\":20,\"rewards\":{\"gold\":200}},{\"place\":21,\"rewards\":{\"gold\":200}},{\"place\":22,\"rewards\":{\"gold\":200}},{\"place\":23,\"rewards\":{\"gold\":200}},{\"place\":24,\"rewards\":{\"gold\":200}},{\"place\":25,\"rewards\":{\"gold\":200}},{\"place\":26,\"rewards\":{\"gold\":200}},{\"place\":27,\"rewards\":{\"gold\":200}},{\"place\":28,\"rewards\":{\"gold\":200}},{\"place\":29,\"rewards\":{\"gold\":200}},{\"place\":30,\"rewards\":{\"gold\":200}},{\"place\":31,\"rewards\":{\"gold\":100}},{\"place\":32,\"rewards\":{\"gold\":100}},{\"place\":33,\"rewards\":{\"gold\":100}},{\"place\":34,\"rewards\":{\"gold\":100}},{\"place\":35,\"rewards\":{\"gold\":100}},{\"place\":36,\"rewards\":{\"gold\":100}},{\"place\":37,\"rewards\":{\"gold\":100}},{\"place\":38,\"rewards\":{\"gold\":100}},{\"place\":39,\"rewards\":{\"gold\":100}},{\"place\":40,\"rewards\":{\"gold\":100}},{\"place\":41,\"rewards\":{\"gold\":50}},{\"place\":42,\"rewards\":{\"gold\":50}},{\"place\":43,\"rewards\":{\"gold\":50}},{\"place\":44,\"rewards\":{\"gold\":50}},{\"place\":45,\"rewards\":{\"gold\":50}},{\"place\":46,\"rewards\":{\"gold\":50}},{\"place\":47,\"rewards\":{\"gold\":50}},{\"place\":48,\"rewards\":{\"gold\":50}},{\"place\":49,\"rewards\":{\"gold\":50}},{\"place\":50,\"rewards\":{\"gold\":50}}]}]";
    
    public static async Task<List<EventData>> GetCurrentEventData()
    {
#if PLUGIN_YG_2

        CurrentEventDataList = new List<EventData>();
        var date = GetDayData(DateTime.Now);

        if (date.Day > 9)
        {
            return CurrentEventDataList;
        }
        else
        {
            var tempList = ParseStringToEventData(STATIC_EVENT_DATA);
            var eventData = tempList[0];
            
            eventData.id = date.Cycle;
            
            Debug.Log(JsonUtility.ToJson(eventData));
            eventData.start_date = GetDay(new EventDayData() { Cycle = date.Cycle, Day = 1 });
            eventData.end_date = GetDay(new EventDayData() { Cycle = date.Cycle, Day = 8 });

            eventData.level_id = new List<int>();
            for (int i = 0; i < 5; i++)
            {
                eventData.level_id.Add(Random.Range(0, EndGameMap.MAX_LEVEL));
            }

            Debug.Log(JsonUtility.ToJson(eventData));
            
            CurrentEventDataList.Add(eventData);

            var crypt = EncryptScore(eventData.id, 100);
            var deCrypt = DecryptScore(crypt);
            
            Debug.Log($"crypt {crypt}   evid {deCrypt.EventId}   sc {deCrypt.Score}" );
            
            return CurrentEventDataList;
        }
#else        
        var request = UnityWebRequest.Get(Model.backend + "/event/current");
        request.SetRequestHeader("Content-Type", ""); // Очищаем
        await request.SendWebRequest();
        switch (request.responseCode)
        {
            case 200:
                CurrentEventDataList = ParseStringToEventData(request.downloadHandler.text);
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
#endif
        return CurrentEventDataList;
    }
    //501 - ошибка при отправке. Ивент окончен 
    
    public static async Task<UpdateResultResponse> PostResultEvent(int eventId, float result)
    {
#if PLUGIN_YG_2

        var currentScore = EncryptScore(eventId, (int)result);

        Debug.Log($"LB SEND current {currentScore}");
        YG2.SetLeaderboard(GetLBName(),currentScore);

        YG2.saves.rewardClaimed = false;
        
        leaderBoardData = null;
        YG2.onGetLeaderboard += GetLBData;
        YG2.GetLeaderboard(GetLBName(),0,1);
        YG2.SaveProgress();
        
        while (leaderBoardData == null)
        {
            await UniTask.Delay(500);
        }
        
        return new UpdateResultResponse()
        {
            place = leaderBoardData.currentPlayer.rank,
            result = DecryptScore(leaderBoardData.currentPlayer.score).Score,
            updated = currentScore == leaderBoardData.currentPlayer.score  
        };
#else
        var request =
 UnityWebRequest.Post(Model.backend + $"event/result?event_id={eventId}", result.ToString(CultureInfo.InvariantCulture), "application/json");
        request.SetRequestHeader("accessToken", Model.GetToken());
        await request.SendWebRequest();
        var resultResponse = new UpdateResultResponse();
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
#endif
    }


#if  PLUGIN_YG_2
    private static LBData leaderBoardData = null;

    private static void GetLBData(LBData lbData)
    {
        if (lbData.technoName != GetLBName()) return;
        leaderBoardData = lbData;
        Debug.Log($"LB:  {JsonUtility.ToJson(lbData)}");
    }

    private static string GetLBName()
    {
        return "mandarin1";
    }

    private static EventDayData GetDayData(DateTime date)
    {
        int dayCount = (date - StartDay).Days;
        return new EventDayData()
        {
            Cycle = (int)(dayCount / 16),
            Day = dayCount % 16
        };
    }
    
    private static DateTime GetDay(EventDayData day)
    {
        return StartDay.AddDays(day.Cycle*16 + day.Day);
    }


    private static int EncryptScore(int eventId, int score)
    {
        int maxScore = 999;
        int resultScore = maxScore - score;

        int eventConst = 1000;
        int resultEventId = eventConst + eventId;

        Debug.Log($"result event id");
        return resultEventId * (1000) + resultScore;
    }
    
    private static ScoreData DecryptScore(int encryptedValue)
    {
        int resultScore = encryptedValue % 1000;
        int resultEventId = encryptedValue / 1000;

        int eventConst = 1000;
        int eventId = resultEventId - eventConst;

        int maxScore = 999;
        int score = maxScore - resultScore;

        return new ScoreData
        {
            EventId = eventId,
            Score = score
        };
    }
    
    private class EventDayData
    {
        public int Cycle;
        public int Day;
    }
    
    
    private class ScoreData
    {
        public int Score;
        public int EventId;
    }
    
#endif
    
    public static async Task<LeaderboardResponse> GetLeaderBoard(int eventId, float limit)
    {
        
        //ТУТ ПИЗДЕЦ. СДЕЛАТЬ ВЫБОРКУ ИЗ СОВПОДАЮШИХ АЙДИ
#if PLUGIN_YG_2
        if (CurrentEventDataList.Count == 0)
        {
            return new();
        }
        
        var currentEvent =  CurrentEventDataList[0];

        YG2.onGetLeaderboard += GetLBData;
        YG2.GetLeaderboard(GetLBName(),(int)limit,1);
        while (leaderBoardData == null)
        {
            await UniTask.Delay(500);
        }

        var result = new LeaderboardResponse();
        result.top = new List<LeaderboardEntry>();
        
        for (int i = 0; i < leaderBoardData.players.Length; i++)
        {
            var player = leaderBoardData.players[i];
            var score = DecryptScore(player.score);
            if (score.EventId == currentEvent.id)
            {
                result.top.Add(new LeaderboardEntry()
                {
                    result = score.Score,
                    place = player.rank,
                    rewards = currentEvent.prizes[player.rank-1].rewards,
                    user_id = 12,
                    user_name = player.name
                    
                });
            }
        }

        if (leaderBoardData.currentPlayer != null &&
            DecryptScore(leaderBoardData.currentPlayer.score).EventId == currentEvent.id)
        {
            var score = DecryptScore(leaderBoardData.currentPlayer.score);
            result.current_user = new LeaderboardEntry()
            {
                result = score.Score,
                place = leaderBoardData.currentPlayer.rank,
                rewards = currentEvent.prizes[leaderBoardData.currentPlayer.rank].rewards,
                user_id = 12,
                user_name = "Вы"
            };
        }
        else
        {
            result.current_user = new LeaderboardEntry()
            {
                result = 0,
                place = 0,
                rewards = new Reward(),
                user_id = 12,
                user_name = "Вы"
            };            
        }

        return result;
#else        
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
#endif
    }

    public static async Task<List<EventReward>> GetUnclaimedPrizes()
    {
        List<EventReward> result = new();
        
#if PLUGIN_YG_2

        if (YG2.saves.rewardClaimed)
        {
            return result;
        }
        
        var day = GetDayData(DateTime.Today);

        if (day.Day <= 7)
        {
            return result;
        }
        
        leaderBoardData = null;
        YG2.onGetLeaderboard += GetLBData;
        YG2.GetLeaderboard(GetLBName(),1,1);
        while (leaderBoardData == null)
        {
            await UniTask.Delay(500);
        }

        var eventData = ParseStringToEventData(STATIC_EVENT_DATA)[0];

        var myScore = DecryptScore(leaderBoardData.currentPlayer.score);
        var firstScore = DecryptScore(leaderBoardData.players[0].score);

        if (firstScore.EventId != myScore.EventId)
        {
            return result;
        }

        if (eventData.prizes.Count > leaderBoardData.currentPlayer.rank)
        {
            var prize = eventData.prizes[leaderBoardData.currentPlayer.rank];

            YG2.saves.rewardClaimed = true;
            YG2.SaveProgress();
            return new List<EventReward>(){ new EventReward()
            {
                event_id = day.Cycle,
                reward_id = 0,
                place = leaderBoardData.currentPlayer.rank,
                rewards = prize.rewards
            }};
        }

        return result;
#else
        var request = UnityWebRequest.Get(Model.backend + $"prizes/unclaimed");
        request.SetRequestHeader("accessToken", Model.GetToken());
        await request.SendWebRequest();
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
#endif
    }
    
    public static async void SetClaimPrize(int id)
    {
#if PLUGIN_YG_2
        YG2.saves.rewardClaimed = true;
        YG2.SaveProgress();
#else        
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
#endif
    }

    private static List<EventData> ParseStringToEventData(string data)
    {
        //Debug.Log("Event: " + data);
        var tempEventDataList = JsonHelper.FromJson<EventDataRaw>(data);
        var eventDataList = new List<EventData>();
        foreach (var tempEventData in tempEventDataList)
        {
            eventDataList.Add(new EventData()
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

        return eventDataList;
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