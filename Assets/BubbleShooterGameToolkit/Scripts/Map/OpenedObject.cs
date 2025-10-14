using System;
using BubbleShooterGameToolkit.Scripts.Audio;
using BubbleShooterGameToolkit.Scripts.CommonUI;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using BubbleShooterGameToolkit.Scripts.System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class OpenedObject : MonoBehaviour
{
    [SerializeField] private Button buy;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private Transform[] gameObjects;
    [SerializeField] private GameObject allObjects;
    [SerializeField] private bool animAllObjects = false;

    [Space] 
    [SerializeField] private Image currencyImage; 

    private AudioClip[] clips;
    private int index = 0;
    private int number;
    private int mapID;
    public delegate void Anim(Vector3 position);
    private Anim anim;

    public void Init(bool isOpened, CostData cost, int num, AudioClip[] clips, int mapID, Anim anim)
    {
        number = num;
        this.mapID = mapID;
        if (isOpened)
        {
            allObjects.SetActive(true);
            buy.gameObject.SetActive(false);
        }
        else
        {
            this.anim=anim;
            allObjects.SetActive(false);
            this.cost.text = cost.Value.ToString();

            switch (cost.CurrencyType)
            {
                case CurrencyType.Coin:
                    currencyImage.sprite= Resources.Load<Sprite>("CurrencySprites/coin");
                    break;
                case CurrencyType.Gem:
                    currencyImage.sprite= Resources.Load<Sprite>("CurrencySprites/gem");
                    break;
            }
            
            buy.gameObject.SetActive(true);
            buy.onClick.RemoveAllListeners();
            buy.onClick.AddListener(Buy);
            this.clips = clips;
        }

        void Buy()
        {
            var currentPrice = 0;
            switch (cost.CurrencyType)
            {
                case CurrencyType.Coin:
                    currentPrice = GameManager.instance.coins.GetResource();
                    break;
                case CurrencyType.Gem:
                    currentPrice = GameManager.instance.gem.GetResource();
                    break;
            }
            
            if (cost.Value > currentPrice)
            {
                if (cost.CurrencyType == CurrencyType.Coin)
                {
                    MenuManager.instance.ShowPopup<CoinsShop>();
                }
                else
                {
                    MenuManager.instance.ShowPopup<GemsShop>();
                }

                return;
            }
            else
            {
                
                //TIDI ДОБАВИТЬ ПРОВЕРКУ
                Model.playerData.endGameMapObjects[mapID][number] = true;
                switch (cost.CurrencyType)
                {
                    case CurrencyType.Coin:
                        GameManager.instance.coins.Consume(cost.Value);
                        break;
                    case CurrencyType.Gem:
                        GameManager.instance.gem.Consume(cost.Value);
                        break;
                }

                AnimObjs();
            }


            void AnimObjs()
            {
                anim(transform.position);
                buy.transform.DOScale(new Vector3(0.5f, 0.3f), 0.5f).OnComplete(() =>
                {
                    
                    buy.gameObject.SetActive(false);
                    PlaySound();
                    allObjects.SetActive(true);
                    if (animAllObjects)
                    {
                        
                        var scale = allObjects.transform.localScale;
                        allObjects.transform.DOScale(new Vector3(1.2f * scale.x, 1.5f*scale.y, 1f*scale.z), 0.3f).OnComplete(() =>
                            allObjects.transform.DOScale(scale, 0.3f).OnComplete(PlaySound));
                    }
                    else
                    {

                        Sequence anim = DOTween.Sequence();
                        foreach (Transform obj in gameObjects)
                        {
                            obj.localScale = new Vector3(0, 0, 0);
                            anim.Append(obj.DOScale(new Vector3(1.2f, 1.5f, 1f), 0.3f).OnComplete(() =>
                                obj.DOScale(new Vector3(1, 1, 1), 0.3f).OnComplete(PlaySound)));

                        }

                        anim.Play();
                    }
                });
            }

            void PlaySound()
            {
                if (clips.Length == 0)
                    return;
                SoundBase.instance.PlaySound(clips[index]);
                index++;
                if (index >= clips.Length)
                    index = 1;
            }
        }


    }

    // private void OnValidate()
    // {
    //     var buyImage = buy.gameObject.GetComponentInChildrenExclusive<Image>();
    //     currencyImage =buyImage.gameObject.GetComponentInChildrenExclusive<Image>();
    // }
}

// public static class ComponentExtensions
// {
//     public static T GetComponentInChildrenExclusive<T>(this GameObject obj, bool includeInactive = false) where T : Component
//     {
//         // Берём всех детей, включая вложенных
//         foreach (Transform child in obj.transform)
//         {
//             var component = child.GetComponentInChildren<T>(includeInactive);
//             if (component != null)
//                 return component;
//         }
//
//         return null;
//     }
// }