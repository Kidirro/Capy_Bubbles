using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using EventData = SpecialEventManager.EventData;

public class EventStartWindow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI startTimeText;
    
    [SerializeField]
    private TextMeshProUGUI eventNameText;

    private EventData _eventData;
    
    public void SetEvent(EventData eventData)
    {
        _eventData = eventData;
        eventNameText.text = eventData.name;
        StartCoroutine(EventTimerProcess());
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
                startTimeText.text = $"{minutesTxt}:{secondsTxt}";
            }
            else
            {
                startTimeText.text = $"{hoursTxt}:{minutesTxt}:{secondsTxt}";
            }

            yield return new WaitForSecondsRealtime(1f);
        }

        //eventHandler.SetActive(false);
    }

}
