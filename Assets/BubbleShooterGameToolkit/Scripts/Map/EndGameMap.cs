using System.Collections.Generic;
using System.Linq;
using System;
using BubbleShooterGameToolkit.Scripts.CommonUI;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using BubbleShooterGameToolkit.Scripts.System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class EndGameMap : MonoBehaviour
{
    [SerializeField] private Button play;
    [SerializeField] private Button nextMap;
    [SerializeField] private Button prevMap;
    [SerializeField] private TMP_Text levels;
    [SerializeField] private Map[] _mapPrefab;
    [SerializeField] private Button rating;
    [SerializeField]
    private AudioClip[] clips;
    [SerializeField] private Transform mapContainer;
    private Map currentMap;
    private static int map =0;
    private int MaxMap => _mapPrefab.Length;
    private List<Map> maps = new List<Map>();
    public const int MAX_LEVEL = 198;
    //198
    public static int LAST_LEVEL => int.Parse(YG2.GetFlag("LastLevel"));

    public const int LAST_LEVEL = 198;

    [SerializeField]
    private Image[] elems;
    public void Open()
    {
        gameObject.SetActive(true);
        if (Model.playerData.counterLevel == 0)
        {
            Model.playerData.counterLevel = LAST_LEVEL;
        }
        levels.text = Model.playerData.counterLevel + " уровень";
        rating.onClick.RemoveAllListeners();
        rating.onClick.AddListener(() => MenuManager.instance.ShowPopup<Rating>());
        levels.text = Model.playerData.counterLevel + " уровень";
        play.onClick.RemoveAllListeners();
        play.onClick.AddListener(PlayRandomLevel);
        maps.Add(Instantiate(_mapPrefab[map], mapContainer));
        currentMap = maps[0];
        currentMap.Init(clips, elems, rating.transform);
        nextMap.onClick.RemoveAllListeners();
        prevMap.onClick.RemoveAllListeners();
        nextMap.onClick.AddListener(NextMap);
        prevMap.onClick.AddListener(PrevMap);
    }


    private void NextMap()
    {
        currentMap.gameObject.SetActive(false);
        map++;
        if (map == MaxMap)
        {
            map = 0;
        }
        int index = maps.FindIndex(x => x.id == _mapPrefab[map].id);
        if (index >= 0)
        {
            maps[index].Init(clips, elems, rating.transform);
            currentMap = maps[index];
        }
        else
        {
            maps.Add(Instantiate(_mapPrefab[map], mapContainer));
            currentMap = maps[map];
            currentMap.Init(clips, elems, rating.transform);
        }
        Debug.Log(GameManager.instance.endGameSetting.mapObjectCost[map].mapObjectCost.Sum());

    }

    private void PrevMap()
    {
        currentMap.gameObject.SetActive(false);
        map--;
        if (map < 0)
        {
            map = MaxMap - 1;
        }
        int index = maps.FindIndex(x => x.id == _mapPrefab[map].id);
        if (index >= 0)
        {
            maps[index].Init(clips,elems, rating.transform);
            currentMap = maps[index];
        }
        else
        {
            maps.Add(Instantiate(_mapPrefab[map], mapContainer));
            currentMap = maps[map];
            currentMap.Init(clips,elems, rating.transform);
        }
    }

    private void PlayRandomLevel()
    {
        if (!GameManager.instance.life.IsEnough(1))
        {
            MenuManager.instance.ShowPopup<LifeShop>();
            return;
        }

        int number = 20 + (Model.playerData.counterLevel * 7 % (MAX_LEVEL - 20));
        PlayerPrefs.SetInt("OpenLevel", number);

        SceneLoader.instance.StartGameScene();
    }
}
