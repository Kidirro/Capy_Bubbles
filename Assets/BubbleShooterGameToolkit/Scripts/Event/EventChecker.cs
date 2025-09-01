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

   
   [SerializeField]
   private RectTransform eventBtnUIParent;

   async private void Start()
   {
      await SpecialEventManager.GetCurrentEventData();

      if (SpecialEventManager.CurrentEventDataList == null || SpecialEventManager.CurrentEventDataList.Count == 0) return;
      
      var eventUI = Instantiate<EventUI>(eventUIprefab, parent);
      eventUI.EventUIContainer = eventBtnUIParent;

   }
}
