using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ProgressChest : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject bubble;
    [SerializeField] private Button chest;
    [SerializeField] private TMP_Text count;
    [SerializeField] private GameObject _light;

    private const int MAX_LEVEL = 50;
    private const int LEVEL_BONUS = 5;
    private int _currentUpgradeLevel = -1;
    private int _currentMaxLevel = -1;
    
    private static int currentLevels = 0;
    private static ChestPrizeSettings ChestPrizeSettings;

    void Awake()
    {
        ChestPrizeSettings = Resources.Load<ChestPrizeSettings>("Settings/ChestPrizeSettings");
        UpgradeDataController.OnUpgradeDataChanged -= OnUpdate;
        UpgradeDataController.OnUpgradeDataChanged += OnUpdate;
    }

    private void OnDestroy()
    {
        UpgradeDataController.OnUpgradeDataChanged -= OnUpdate;
    }
    void OnEnable()
    {
        UpdateUI();
    }

    private async void UpdateUI()
    {
        await LoadUpgradeLevel();
        _currentMaxLevel = MAX_LEVEL - _currentUpgradeLevel * LEVEL_BONUS;
        
        currentLevels = PlayerPrefs.GetInt("ChestLevels", 0);
        slider.value = (float)currentLevels / _currentMaxLevel;
        if (currentLevels >= _currentMaxLevel)
        {
            slider.handleRect.gameObject.SetActive(false);
            chest.onClick.AddListener(OpenChest);
            bubble.SetActive(false);
            _light.SetActive(true);
        }
        else
        {
            slider.handleRect.gameObject.SetActive(true);
            bubble.SetActive(true);
            _light.SetActive(false);
            chest.onClick.AddListener(ShowCount);
        }
    }

    void OnDisable()
    {
        chest.onClick.RemoveAllListeners();
    }

    private void ShowCount()
    {
        count.text = currentLevels + "/" + _currentMaxLevel;
        count.transform.parent.gameObject.SetActive(true);
        StartCoroutine(HideCount());
    }

    private IEnumerator HideCount()
    {
        yield return new WaitForSeconds(2);
        count.transform.parent.gameObject.SetActive(false);

    }
    private void OpenChest()
    {
        PlayerPrefs.SetInt("ChestLevels", 0);
        slider.value = 0;
        currentLevels = 0;
        chest.onClick.RemoveAllListeners();
        chest.onClick.AddListener(ShowCount);
        slider.handleRect.gameObject.SetActive(true);
        bubble.SetActive(true);
        _light.SetActive(false);
        int randomPrize = Random.Range(0, ChestPrizeSettings.rewardVisuals.Length);
        BubbleShooterGameToolkit.Scripts.CommonUI.
                MenuManager.instance.ShowPopup<RewardPopup>().SetReward(ChestPrizeSettings.rewardVisuals[randomPrize].rewardVisuals);
    }

    private async UniTask LoadUpgradeLevel()
    {
        while (Model.playerData == null)
        {
            await UniTask.Delay(100);
        }

        _currentUpgradeLevel = UpgradeDataController.GetUpgradeLevel("ChestLevelBonus");
    }

    private void OnUpdate(UpgradeData obj)
    {
        if (obj.UpgradeId != "ChestLevelBonus") return;
        UpdateUI();
    }


}
