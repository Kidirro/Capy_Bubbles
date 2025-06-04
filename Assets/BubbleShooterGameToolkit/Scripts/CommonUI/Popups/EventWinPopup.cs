using System;
using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using BubbleShooterGameToolkit.Scripts.Gameplay.Managers;
using BubbleShooterGameToolkit.Scripts.LevelSystem;
using BubbleShooterGameToolkit.Scripts.Settings;
using BubbleShooterGameToolkit.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventWinPopup : Popup
{
    //[SerializeField] private GameObject[] stars;
    //[SerializeField] private GameObject[] glitters;
    //[SerializeField] private GameObject[] starEffets;
    [SerializeField] private Button play;
    [SerializeField] private Button toMenu;
    
    //[SerializeField] private Button restart;
    [SerializeField] private TextMeshProUGUI scoreText;

    private int currentLevelEventId = 0;
    
    public void OnEnable () {
        // play.onClick.AddListener(GoMap);
        // closeButton.onClick.AddListener(GoMap);
        // restart.onClick.AddListener(RestartLevel);
        scoreText.text = LevelManager.instance.EventTime.ToString();
    }

    private void Start()
    {
        currentLevelEventId = PlayerPrefs.GetInt("EventLevel", 0) + 1;
        PlayerPrefs.SetInt("EventLevel", currentLevelEventId);


        if (currentLevelEventId == SpecialEventManager.ChosenEventData.level_id.Count)
        {
            toMenu.gameObject.SetActive(true);
            play.gameObject.SetActive(false);
        }
        else
        {
            play.gameObject.SetActive(true);
            toMenu.gameObject.SetActive(false);
        }
    }

    public void NextLevel()
    {
        int currentLevel =SpecialEventManager.ChosenEventData.level_id[currentLevelEventId];
        PlayerPrefs.SetInt("OpenLevel", currentLevel);
        PlayerPrefs.SetInt("OpenEvent", 1);
    }

    public void GoMap()
    {
        Close();
        var gameSettings = Resources.Load<GameSettings>("Settings/GameSettings");
        if(!gameSettings.GoMapAfter.skipMap || LevelLoader.instance.CurrentLevel.Number >= gameSettings.GoMapAfter.untilLevel)
            OnCloseAction =(_) => SceneLoader.instance.GoToMap();
        else
        {
            PlayerPrefs.SetInt("OpenLevel", PlayerPrefs.GetInt("Level", 1));
            OnCloseAction = (_) => SceneLoader.instance.StartGameScene();
        }
    }

    public override void AfterShowAnimation()
    {
        base.AfterShowAnimation();
			
            //   AnimateStars();
    }
    
}
