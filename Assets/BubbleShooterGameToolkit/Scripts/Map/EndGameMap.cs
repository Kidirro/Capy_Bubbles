using BubbleShooterGameToolkit.Scripts.System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGameMap : MonoBehaviour
{
    [SerializeField] private Button play;
    [SerializeField] private TMP_Text levels;
    public const int MAX_LEVEL = 198;
    public const int LAST_LEVEL = 198;
    public void Open()
    {
        gameObject.SetActive(true);
        if(Model.playerData.counterLevel==0)
        {
            Model.playerData.counterLevel=LAST_LEVEL;
        }
        levels.text =  Model.playerData.counterLevel.ToString();
        play.onClick.AddListener(PlayRandomLevel);
    }

    private void PlayRandomLevel()
    {
        int number = 20+(Model.playerData.counterLevel*7%(MAX_LEVEL-20));
        PlayerPrefs.SetInt("OpenLevel", number);
        SceneLoader.instance.StartGameScene();
    }
}
