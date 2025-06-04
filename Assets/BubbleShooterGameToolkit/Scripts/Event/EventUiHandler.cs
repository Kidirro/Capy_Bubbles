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
    
    private EventData _eventData;

    public void SetEvent(EventData eventData)
    {
        _eventData = eventData;
        StartCoroutine(EventTimerProcess());
    }
    
    private IEnumerator EventTimerProcess()
    {
        float timeRemaining = 0;
        
        float timeStart = (float)(_eventData.start_date - DateTime.UtcNow).TotalSeconds;
        float timeEnd = (float)(DateTime.UtcNow - _eventData.end_date).TotalSeconds;
        
        if (timeStart > 0)
        {
            timeRemaining = timeStart * 60f;
        }
        else
        {
            timeRemaining = timeEnd * 60f;
        }

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;

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

            yield return null;
        }

        //eventHandler.SetActive(false);
    }

    public void OpenEventWindow()
    {
        SpecialEventManager.SetChosenEventData(_eventData);
    }
}
