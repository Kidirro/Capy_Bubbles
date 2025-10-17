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
   
   [SerializeField]
   private GameObject arrowUp;
   
   [SerializeField]
   private GameObject arrowDown;

   private async void Start()
   {
      var currentScore = PlayerPrefs.GetFloat($"EventTime{SpecialEventManager.ChosenEventData.id}", 0);
      var postResultEvent =  await SpecialEventManager.PostResultEvent(SpecialEventManager.ChosenEventData.id, currentScore);
      

      timeText.text = ((int)postResultEvent.result).ToString() + " сек";
      placeText.text = postResultEvent.place + " место";
      
      arrowUp.SetActive(postResultEvent.updated);
      arrowDown.SetActive(!postResultEvent.updated);
   }
}
