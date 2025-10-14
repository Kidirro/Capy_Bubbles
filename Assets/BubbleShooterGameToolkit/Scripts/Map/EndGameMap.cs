using System;
using System.Collections.Generic;
using System.Linq;
using BubbleShooterGameToolkit.Scripts.CommonUI;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using BubbleShooterGameToolkit.Scripts.System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class EndGameMap : MonoBehaviour
{
    [SerializeField] private Button play;
    [SerializeField] private Button nextMap;
    [SerializeField] private Button prevMap;
    [SerializeField] private TMP_Text levels;
    [SerializeField] private Button rating;
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private Transform mapContainer;
    
    
    [Space,SerializeField] private CanvasGroup loadingCanvas;
    [SerializeField] private Image loadingBar;
    private Map currentMap;
    private static int map =0;
    
    private bool _isLoading = false;

    private List<string> mapPrefabNames = new List<string>()
    {
        "Map_1",
        "Map_2",
        "Map_3",
        "Map_4", 
        "Map_5"
    };
    
    private static Dictionary<int, Map> mapPrefabs = new Dictionary<int, Map>();
    
    private int MaxMap => mapPrefabNames.Count;
    public const int MAX_LEVEL = 198;
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
        rating.onClick.RemoveAllListeners();
        rating.onClick.AddListener(() => MenuManager.instance.ShowPopup<Rating>());
        levels.text = Model.playerData.counterLevel + " уровень";
        play.onClick.RemoveAllListeners();
        play.onClick.AddListener(PlayRandomLevel);
        ShowMap(map);
        nextMap.onClick.RemoveAllListeners();
        prevMap.onClick.RemoveAllListeners();
        nextMap.onClick.AddListener(NextMap);
        prevMap.onClick.AddListener(PrevMap);
    }


    private void NextMap()
    {
        if (_isLoading) return;
        map++;
        if (map == MaxMap)
        {
            map = 0;
        }
        
        ShowMap(map);

    }

    private void PrevMap()
    {
        if (_isLoading) return;
        map--;
        if (map < 0)
        {
            map = MaxMap - 1;
        }
        ShowMap(map);
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
        PlayerPrefs.SetInt("OpenEvent", 0);

        SceneLoader.instance.StartGameScene();
    }

    private async void ShowMap(int id)
    {
        //Показ окна загрузки

        //loadingCanvas.DoFadeOut();

        _isLoading = true;

        if (mapPrefabs.ContainsKey(id) == false)
        {

            loadingCanvas.gameObject.SetActive(true);
            loadingCanvas.DOFade(1, 0.1f);
            loadingBar.fillAmount = 0f;
            await UniTask.Delay(100);

            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(mapPrefabNames[id]);

            while (handle.IsDone == false)
            {
                loadingBar.DOFillAmount(handle.PercentComplete, 0.25f);
                await UniTask.Delay(250);
            }

            loadingBar.DOFillAmount(1, 0.1f);
            await UniTask.Delay(100);

            
            mapPrefabs.Add(id, handle.Result.GetComponent<Map>());
                
            loadingCanvas.DOFade(0, 0.1f);
            await UniTask.Delay(100);
            loadingCanvas.gameObject.SetActive(false);
        }

        if (currentMap != null)
        {
            Destroy(currentMap.gameObject);
        }
        
        currentMap = mapPrefabs[id];
        currentMap = Instantiate(mapPrefabs[id], mapContainer);
        currentMap.Init(clips, elems, rating.transform);
        

        _isLoading = false;
        //Стоп загрузки
    }
    
#if UNITY_EDITOR
    private void OnApplicationQuit()
    {
        mapPrefabs.Clear();
        map = 0;
    }
#endif    
}
