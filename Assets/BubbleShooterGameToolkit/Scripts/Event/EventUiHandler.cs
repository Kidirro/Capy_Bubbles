using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EventData = SpecialEventManager.EventData;

public class EventUiHandler : MonoBehaviour
{
  
    [SerializeField] 
    private Image eventImage;
    
    [SerializeField]
    private TextMeshProUGUI eventText;
    
    [SerializeField]
    private List<ImageData> eventImageList;
    
    private Dictionary<int, Sprite> _eventImageDictionary = new ();
    private EventData _eventData;

    public void SetEvent(EventData eventData)
    {
        _eventData = eventData;
        PrepareSprites();
        if (_eventImageDictionary.TryGetValue(_eventData.logo, out var sprite))
        {
            eventImage.sprite = sprite;
        }

        StartCoroutine(EventTimerProcess());
    }

    private void PrepareSprites()
    {
        if (_eventImageDictionary.Count == eventImageList.Count) return;
        
        _eventImageDictionary.Clear();
        foreach (var imageData in eventImageList)
        {
            _eventImageDictionary[imageData.id] = imageData.sprite; 
        }
    }
    
    private IEnumerator EventTimerProcess()
    {
        var timeRemaining = 0f;
        
        var timeStart = (float)((_eventData.start_date - DateTime.Now).TotalSeconds);
        var timeEnd = (float)((_eventData.end_date - DateTime.Now).TotalSeconds);
        
        if (timeStart > 0)
        {
            timeRemaining = timeStart;
        }
        else
        {
            timeRemaining = timeEnd;
        }
        

        while (timeRemaining > 0)
        {
            timeRemaining -= 1;

            float seconds = Mathf.FloorToInt(timeRemaining % 60);
            float minutes = Mathf.FloorToInt(timeRemaining / 60);
            float hours = Mathf.FloorToInt(minutes / 60);

            if (hours > 0)
                minutes = Mathf.FloorToInt(minutes % 60);

            string minutesTxt = minutes.ToString();
            string secondsTxt = seconds.ToString();
            string hoursTxt = hours.ToString();

            if (minutes < 10)
                minutesTxt = $"0{minutes}";

            if (seconds < 10)
                secondsTxt = $"0{seconds}";

            if (hours < 10)
                hoursTxt = $"0{hours}";

            if (hours < 1)
            {
                eventText.text = $"{minutesTxt}:{secondsTxt}";
            }
            else
            {
                eventText.text = $"{hoursTxt}:{minutesTxt}:{secondsTxt}";
            }

            yield return new WaitForSecondsRealtime(1f);
        }

        if (SpecialEventManager.ChosenEventData == _eventData)
        {
            EventUI.instance.gameObject.SetActive(false);
            EventUI.instance.CheckClaimRewards();
        }

        eventText.text = "Результаты";

    }

    public void OpenEventWindow()
    {
        SpecialEventManager.SetChosenEventData(_eventData);
        EventUI.instance.ShowEventWindow();
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
    
    [Serializable]
    public class ImageData
    {
        public int id;
        public Sprite sprite;
    }
}
