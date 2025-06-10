using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUIRating : MonoBehaviour
{
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
    async void Start()
    {
        loadingScreen.gameObject.SetActive(true);
        var leaderboard = await SpecialEventManager.GetLeaderBoard(SpecialEventManager.ChosenEventData.id,10);
        loadingScreen.gameObject.SetActive(false);
        
        _playerCards.Clear();
        foreach (var leaderboardEntry in leaderboard.top)
        {
            var ratingCard = Instantiate(playerCardPrefab, playerCardContainer);
            ratingCard.SetLeaderboardEntry(leaderboardEntry);
            _playerCards.Add(ratingCard);
        }
        
        nullRatingWarning.gameObject.SetActive(leaderboard.top.Count == 0);

        leaderboard.current_user.user_name = Model.playerData.phone;
        currentPlayerCard.SetLeaderboardEntry(leaderboard.current_user);
    }

    public void ClearLeaderBoard()
    {
        while (_playerCards.Count>0)
        {
            var playerCard = _playerCards[0];
            _playerCards.Remove(playerCard);
            Destroy(playerCard);
        }
    }
}
