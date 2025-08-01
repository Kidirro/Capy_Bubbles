using BubbleShooterGameToolkit.Scripts.Audio;
using BubbleShooterGameToolkit.Scripts.CommonUI;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using BubbleShooterGameToolkit.Scripts.System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpenedObject : MonoBehaviour
{
    [SerializeField] private Button buy;
    [SerializeField] private TMP_Text cost;
    [SerializeField] private Transform[] gameObjects;
    [SerializeField] private GameObject allObjects;
    [SerializeField] private bool animAllObjects = false; 

    private AudioClip[] clips;
    private int index = 0;
    private int number;
    private int mapID;
    public delegate void Anim(Vector3 position);
    private Anim anim;

    public void Init(bool isOpened, int cost, int num, AudioClip[] clips, int mapID, Anim anim)
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
            this.cost.text = cost.ToString();
            buy.gameObject.SetActive(true);
            buy.onClick.RemoveAllListeners();
            buy.onClick.AddListener(Buy);
            this.clips = clips;
        }

        void Buy()
        {
            if (cost > GameManager.instance.coins.GetResource())
            {
                MenuManager.instance.ShowPopup<CoinsShop>();
                return;
            }
            else
            {
                
                //TIDI ДОБАВИТЬ ПРОВЕРКУ
                Model.playerData.endGameMapObjects[mapID][number] = true;
                GameManager.instance.coins.Consume(cost);

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
                        allObjects.transform.DOScale(new Vector3(1.2f, 1.5f, 1f), 0.3f).OnComplete(() =>
                            allObjects.transform.DOScale(new Vector3(1, 1, 1), 0.3f).OnComplete(PlaySound));
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
}
