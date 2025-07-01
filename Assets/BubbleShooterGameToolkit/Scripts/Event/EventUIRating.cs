using System;
using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using TMPro;
using UnityEngine;

public class EventUIRating : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI ratingText;
    
    [SerializeField]
    private GameObject loadingScreen;
    
    [SerializeField]
    private EventUIRatingPlayerCard playerCardPrefab;
    
    [SerializeField]
    private Transform playerCardContainer;

    [SerializeField]
    private GameObject nullRatingWarning;
    
    [Space, SerializeField]
    private EventUIRatingPlayerCard currentPlayerCard;
    
    private List<EventUIRatingPlayerCard> _playerCards = new List<EventUIRatingPlayerCard>();

    public async void Show()
    {
        
        gameObject.SetActive(true);
        loadingScreen.gameObject.SetActive(true);
        var leaderboard = await SpecialEventManager.GetLeaderBoard(SpecialEventManager.ChosenEventData.id,10);

        if (leaderboard == null)
        {
            gameObject.SetActive(false);
            return;
        }
        
        loadingScreen.gameObject.SetActive(false);
        
        _playerCards.Clear();
        foreach (var leaderboardEntry in leaderboard.top)
        {
            var ratingCard = Instantiate(playerCardPrefab, playerCardContainer);
            ratingCard.SetLeaderboardEntry(leaderboardEntry);
            _playerCards.Add(ratingCard);
        }
        
        nullRatingWarning.gameObject.SetActive(leaderboard.top.Count == 0);


        /*
        switch (SpecialEventManager.ChosenEventData.event_type)
        {
            case SpecialEventManager.EventType.time:
                ratingText.text = "Время";
                break;
            case SpecialEventManager.EventType.score:
                ratingText.text = "Очки";
                break;
                
        }
        */
        
        leaderboard.current_user.user_name = Model.playerData.phone;
        currentPlayerCard.SetLeaderboardEntry(leaderboard.current_user);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        ClearLeaderBoard();
    }

    public void ClearLeaderBoard()
    {
        while (_playerCards.Count>0)
        {
            var playerCard = _playerCards[0];
            _playerCards.Remove(playerCard);
            Destroy(playerCard.gameObject);
        }
    }
}
