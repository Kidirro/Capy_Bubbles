using System;
using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.CommonUI;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using BubbleShooterGameToolkit.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Obsolete("Нужен рефакторинг")]
public class EventUI : SingletonBehaviour<EventUI>
{
    private Transform eventUIContainer;
    
    public Transform EventUIContainer
    {
        get => eventUIContainer;
        set => eventUIContainer = value;
    }

    [SerializeField]
    private EventUiHandler eventUiHandler;

    [SerializeField] 
    private GameObject eventWindow;
    
    [SerializeField]
    private TextMeshProUGUI eventProgressText;
    
    [SerializeField]
    private Image eventProgressImage;
    
    [SerializeField]
    private Image mandarinProgressImage;

    [SerializeField]
    private TextMeshProUGUI eventNameText;

    [Space, SerializeField]
    private GameObject eventProgressHandler;
    
    [SerializeField]
    private GameObject eventCompleteHandler;
    
    [Space, SerializeField]
    private EventUIRatingPlayerCard ratingPlayerCard;
    
    [Space, SerializeField]
    private EventCompleteWindow eventCompleteWindow;
    
    [Space, SerializeField]
    private EventStartWindow eventStartWindow;
    
    [Space, SerializeField]
    private EventClaimPrizesWindow eventClaimPrizesWindow;
    
    [Space, SerializeField]
    private EventUIRating eventUIRating;
    
    async void Start()
    {
        await SpecialEventManager.GetCurrentEventData();

        if (SpecialEventManager.CurrentEventDataList == null) return;

        foreach (var eventData in SpecialEventManager.CurrentEventDataList)
        {
            if (eventData.id < 0) continue;
            var eventObject = Instantiate(eventUiHandler, eventUIContainer);

            eventObject.SetEvent(eventData);
            eventObject.transform.SetSiblingIndex(0);

        }

        var isEventPlayed = PlayerPrefs.GetInt("OpenEvent", 0);


        if (isEventPlayed == 1 && SpecialEventManager.ChosenEventData != null)
        {
            ShowEventWindow();
            int currentLevelEventId = PlayerPrefs.GetInt("EventLevel" + SpecialEventManager.ChosenEventData.id, 0);
            var eventComplete = currentLevelEventId == SpecialEventManager.ChosenEventData.level_id.Count;

            if (eventComplete)
            {
                eventCompleteWindow.gameObject.SetActive(true);
            }
        }

        PlayerPrefs.SetInt("OpenEvent", 0);
        CheckClaimRewards();
    }

    public void ReplyTry()
    {
        bool isInProgress = 
            (DateTime.Now -SpecialEventManager.ChosenEventData.start_date).TotalSeconds>0 &&
            (DateTime.Now -SpecialEventManager.ChosenEventData.end_date).TotalSeconds < 0;

        if (isInProgress == false)
        {
            eventCompleteWindow.gameObject.SetActive(false);
            gameObject.SetActive(false);
            return;
        }
        if (GameManager.instance.gem.Consume(GameManager.instance.GameSettings.eventReplyCost))
        {
            PlayerPrefs.SetInt("EventLevel" + SpecialEventManager.ChosenEventData.id, 0);
            PlayerPrefs.SetFloat($"EventTime{SpecialEventManager.ChosenEventData.id}", 0);
            StartEventLevel();
        }
    }

    public void StartEventLevel()
    {
        
        bool isInProgress = 
            (DateTime.Now -SpecialEventManager.ChosenEventData.start_date).TotalSeconds>0 &&
            (DateTime.Now -SpecialEventManager.ChosenEventData.end_date).TotalSeconds < 0;

        if (isInProgress == false)
        {
            gameObject.SetActive(false);
            return;
        }
        
        if (!GameManager.instance.life.IsEnough(1))
        {
            MenuManager.instance.ShowPopup<LifeShop>();
        }
        else
        {

            int currentLevelEventId = PlayerPrefs.GetInt("EventLevel" + SpecialEventManager.ChosenEventData.id, 0);
            int currentLevel = SpecialEventManager.ChosenEventData.level_id[currentLevelEventId];
            PlayerPrefs.SetInt("OpenLevel", currentLevel);
            PlayerPrefs.SetInt("OpenEvent", 1);

            SceneLoader.instance.StartGameScene();
        }
    }

    public void ShowEventWindow()
    {
        bool isInProgress = 
            (DateTime.Now -SpecialEventManager.ChosenEventData.start_date).TotalSeconds>0 &&
            (DateTime.Now -SpecialEventManager.ChosenEventData.end_date).TotalSeconds < 0;
        
        bool isAwaitEvent = (DateTime.Now - SpecialEventManager.ChosenEventData.start_date).TotalSeconds < 0;
        
        if (isInProgress)
        {

            int currentLevelEventId = PlayerPrefs.GetInt("EventLevel" + SpecialEventManager.ChosenEventData.id, 0);

            var eventInProgress = currentLevelEventId < SpecialEventManager.ChosenEventData.level_id.Count;

            eventProgressHandler.gameObject.SetActive(eventInProgress);
            eventCompleteHandler.gameObject.SetActive(!eventInProgress);

            if (eventInProgress)
            {

                var progressRatio =
                    ((float)currentLevelEventId) / (float)SpecialEventManager.ChosenEventData.level_id.Count;
                eventProgressImage.fillAmount = progressRatio;

                var mandarinPosition = -300 + 600 * progressRatio;
                mandarinProgressImage.transform.localPosition =
                    new Vector3(mandarinPosition, mandarinProgressImage.transform.localPosition.y);

                eventProgressText.text = currentLevelEventId + "/" + SpecialEventManager.ChosenEventData.level_id.Count;

            }
            else
            {
                UpdateUICard();
            }

            eventNameText.text = SpecialEventManager.ChosenEventData.name;
            eventWindow.SetActive(true);
        }
        else if (isAwaitEvent)
        {
            eventStartWindow.gameObject.SetActive(true);
            eventStartWindow.SetEvent(SpecialEventManager.ChosenEventData);
        }
        else
        {
            eventUIRating.Show();
        }
    }

    private async void UpdateUICard()
    {
        var leaderboard = await SpecialEventManager.GetLeaderBoard(SpecialEventManager.ChosenEventData.id,1);
        leaderboard.current_user.user_name = Model.playerData.phone;
        ratingPlayerCard.SetLeaderboardEntry(leaderboard.current_user);
    }

    public void CheckClaimRewards()
    {
        StartCoroutine(ClaimRewards());
    }
    
    private IEnumerator ClaimRewards()
    {
        var task = SpecialEventManager.GetUnclaimedPrizes();
        while (task.IsCompleted == false)
        {
            yield return null;
        }

        var rewards = task.Result;

        foreach (var reward in rewards)
        {
            eventClaimPrizesWindow.gameObject.SetActive(true);
            eventClaimPrizesWindow.ShowReward(reward);

            while (eventClaimPrizesWindow.gameObject.activeSelf)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
