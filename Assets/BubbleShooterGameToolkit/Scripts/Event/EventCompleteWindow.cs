using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EventCompleteWindow : MonoBehaviour
{
   [SerializeField]
   private TextMeshProUGUI timeText;

   [SerializeField] 
   private TextMeshProUGUI placeText;


   private async void Start()
   {
      var leaderboard = await SpecialEventManager.GetLeaderBoard(SpecialEventManager.ChosenEventData.id, 1);

      timeText.text = leaderboard.current_user.result + " сек";
      placeText.text = leaderboard.current_user.place + " место";
   }
}
