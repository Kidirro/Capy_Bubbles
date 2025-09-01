using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventChecker : MonoBehaviour
{
   [SerializeField]
   private RectTransform parent;
   
   [SerializeField]
   private EventUI eventUIprefab;


   async private void Start()
   {
      await SpecialEventManager.GetCurrentEventData();

      if (SpecialEventManager.CurrentEventDataList == null) return;
      
      Instantiate(eventUIprefab, parent);

   }
}
