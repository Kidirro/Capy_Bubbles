using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LeaderboardEntry = SpecialEventManager.LeaderboardEntry; 

public class EventUIRatingPlayerCard : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI placeText;

    [SerializeField]
    private TextMeshProUGUI nameText;
    
    [SerializeField]
    private TextMeshProUGUI scoreText;
    
    [SerializeField]
    private List<GameObject> placeIcons = new List<GameObject>();
    
    private LeaderboardEntry _leaderboardEntry;

    public void SetLeaderboardEntry(LeaderboardEntry leaderboardEntry)
    {
        _leaderboardEntry = leaderboardEntry;
        UpdateUIRatingPlayerCard();
    }

    public void UpdateUIRatingPlayerCard()
    {
        placeText.text = _leaderboardEntry.place == 0 ? "-" : _leaderboardEntry.place.ToString();
        nameText.text = _leaderboardEntry.user_name.ToString();
        scoreText.text = ((int)_leaderboardEntry.result).ToString();
        for (int i = 0; i < placeIcons.Count; i++)
        {
            placeIcons[i].SetActive(i + 1 == _leaderboardEntry.place);
        }
    }
}
