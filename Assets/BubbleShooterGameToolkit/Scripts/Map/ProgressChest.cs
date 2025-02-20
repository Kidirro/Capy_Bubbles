using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressChest : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject bubble;
    [SerializeField] private Button chest;
    [SerializeField] private TMP_Text count;
    [SerializeField] private GameObject _light;

    private const int MAX_LEVEL = 20;
    private static int currentLevels = 0;
    private static ChestPrizeSettings ChestPrizeSettings;

    void Awake()
    {
        ChestPrizeSettings = Resources.Load<ChestPrizeSettings>("Settings/ChestPrizeSettings");
    }

    void OnEnable()
    {
        currentLevels = PlayerPrefs.GetInt("ChestLevels", 0);
        slider.value = (float)currentLevels / MAX_LEVEL;
        if (currentLevels >= MAX_LEVEL)
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
        count.text = currentLevels + "/" + MAX_LEVEL;
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

}
