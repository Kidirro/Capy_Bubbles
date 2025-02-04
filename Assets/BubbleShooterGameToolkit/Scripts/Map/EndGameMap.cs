using BubbleShooterGameToolkit.Scripts.CommonUI;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using BubbleShooterGameToolkit.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMap : MonoBehaviour
{
    [SerializeField] private Button play;
    [SerializeField] private TMP_Text levels;
    [SerializeField] private OpenedObject[] openedObjects;
    [SerializeField] 
    private AudioClip[] clips;
    public const int MAX_LEVEL = 198;
    public const int LAST_LEVEL = 3;
    public void Open()
    {
        gameObject.SetActive(true);
        if (Model.playerData.counterLevel == 0)
        {
            Model.playerData.counterLevel = LAST_LEVEL;
        }
        levels.text = Model.playerData.counterLevel+" уровень";
        play.onClick.AddListener(PlayRandomLevel);
        for (int i = 0; i < openedObjects.Length; i++)
        {
            OpenedObject openedObject = openedObjects[i];
            openedObjects[i].Init(Model.playerData.endGameFirstMapObjectsOpen[i], GameManager.instance.endGameSetting.mapObjectCost[i], i, clips);
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
