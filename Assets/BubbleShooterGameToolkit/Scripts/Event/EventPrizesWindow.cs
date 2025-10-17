using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventPrizesWindow : MonoBehaviour
{
  [SerializeField] private Transform prizesCardContainer;

  [SerializeField] private PrizeCard prizeCardPrefab;

  void Start()
  {
    var prizes = SpecialEventManager.ChosenEventData.prizes;
    foreach (var prize in prizes)
    {
      var obj = Instantiate(prizeCardPrefab, prizesCardContainer);
      obj.SetPrize(prize);
      obj.gameObject.SetActive(true);
    }
  }
}
