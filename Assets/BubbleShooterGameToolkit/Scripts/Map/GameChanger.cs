using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.Map;
using UnityEngine;

public class GameChanger : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private EndGameMap mapEndGame;

    private void Start()
    {
        Debug.Log("Model.playerData.levels.Count"+Model.playerData.levels.Count);
        if(Model.playerData.levels.Count>=EndGameMap.LAST_LEVEL)
        {
            mapEndGame.Open();
        }
        else
        {
            Debug.Log("MAP MENAANAN");
            mapManager.Open();
        }
    }
}
