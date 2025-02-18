using System.Threading.Tasks;
using System.Xml.Schema;
using BubbleShooterGameToolkit.Scripts.System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{

    private const float MIN_ITEMS_FLY_TIME = 0.8f;
    private const float MAX_ITEMS_FLY_TIME = 1.5f;

    private const float ITEMS_EXPLOSION_TIME = 0.28f;
    private const float REDUCE_SCALE_OF_ITEM_AMOUNT = 0.1f;
    [SerializeField] private OpenedObject[] openedObjects;
    public int id;
    private Image[] elems;
    private Vector2 ratingPosistion;
    private Transform ratingTransform;
    public void Init(AudioClip[] clips, Image[] elems, Transform rating)
    {
        bool[] bools;
        this.elems = elems;
        ratingPosistion = rating.position;
        ratingTransform=rating;
        switch (id)
        {
            case 0:
                bools = Model.playerData.endGameFirstMapObjectsOpen;
                break;
            case 1:
                bools = Model.playerData.endGameSecondMapObjectsOpen;

                break;
            default:
                bools = new bool[100];
                break;
        }
        gameObject.SetActive(true);
        for (int i = 0; i < openedObjects.Length; i++)
        {
            openedObjects[i].Init(bools[i], GameManager.instance.endGameSetting.mapObjectCost[id].mapObjectCost[i], i, clips, id, CreateAnim);
        }
    }
    Sequence flySequence;
    Sequence sequence;
    private async void CreateAnim(Vector3 startPosition)
    {

        foreach (Image element in elems)
        {
            element.transform.position = startPosition;
            element.transform.localScale = Vector3.one;
            element.gameObject.SetActive(true);
        }
        flySequence = CreateExplosionSequence();
        sequence = DOTween.Sequence();
        sequence.Pause();
        flySequence.Pause();


        int itemCount = elems.Length;
        Vector2 finalPosition = ratingPosistion;
        for (int i = 0; i < elems.Length; i++)
        {
            int j = i;
            Vector2 pathPoint = RandomizePosition(finalPosition, -0.1f, 0.1f, -0.4f, -0.2f);

            float moveTime = Random.Range(MIN_ITEMS_FLY_TIME, MAX_ITEMS_FLY_TIME);
            sequence.Join(elems[j].transform.DOPath(new Vector3[] { pathPoint, finalPosition }, moveTime, PathType.CatmullRom)
            .OnComplete(() => { ratingTransform.DOPunchScale(new Vector3(0.2f,0.2f,0.2f),0.1f); elems[j].gameObject.SetActive(false); }));
        }

        flySequence.Play();
        await flySequence.AsyncWaitForCompletion();
        sequence.Play();
        await sequence.AsyncWaitForCompletion();

        ratingTransform.localScale= Vector3.one;
    }

    protected virtual Sequence CreateExplosionSequence()
    {
        Sequence sequence = DOTween.Sequence();
        foreach (Image element in elems)
        {
            Transform elementTransform = element.transform;
            elementTransform.position = RandomizePosition(elementTransform.position);

            Vector2 elementStartPos = RandomizePosition(elementTransform.position);
            Vector2 endScaleAmount = (Vector2)elementTransform.localScale - Vector2.one * REDUCE_SCALE_OF_ITEM_AMOUNT;

            sequence.Join(elementTransform.DOMove(elementStartPos, ITEMS_EXPLOSION_TIME))
            .Join(elementTransform.DOScale(endScaleAmount, ITEMS_EXPLOSION_TIME - 0.1f).SetEase(Ease.InCirc));
        }
        return sequence;
    }
    protected Vector2 RandomizePosition(Vector2 targetPosition, float xRandomBottom = 0.7f, float xRandomTop = 0.7f, float yRandomBottom = 0.7f, float yRandomTop = 0.7f) =>
    targetPosition + new Vector2(Random.Range(-xRandomBottom, xRandomTop), Random.Range(-yRandomBottom, yRandomTop));
}