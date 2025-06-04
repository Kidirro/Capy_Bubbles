using System;
using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
    //[SerializeField]
    //private GameObject eventHandler;
    
    [SerializeField]
    private Transform eventUIContainer;
    
    [SerializeField]
    private EventUiHandler eventUiHandler;
    
    async void Start()
    {
        await SpecialEventManager.GetCurrentEventData();

        if (SpecialEventManager.CurrentEventDataList == null) return;
        
        foreach (var eventData in SpecialEventManager.CurrentEventDataList)
        {
            var isStarted = (DateTime.UtcNow - eventData.start_date).Seconds > 0;
            var isInProgress = (eventData.end_date - DateTime.UtcNow).Seconds > 0;

            if (isStarted && isInProgress)
            {
                var eventObject = Instantiate(eventUiHandler, eventUIContainer);
                    
                eventObject.SetEvent(eventData);
                eventObject.transform.SetSiblingIndex(0);
            }
        }
    }

    public void StartEventLevel()
    {
        int currentLevelEventId = PlayerPrefs.GetInt("EventLevel", 0);
        int currentLevel = 0;//SpecialEventManager.CurrentEventData.level_id[currentLevelEventId];
        PlayerPrefs.SetInt("OpenLevel", currentLevel);
        PlayerPrefs.SetInt("OpenEvent", 1);

        SceneLoader.instance.StartGameScene();
    }
}
