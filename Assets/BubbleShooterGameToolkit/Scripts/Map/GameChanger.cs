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
        Debug.Log($"[DEBUG] Model.playerData {Model.playerData}.");
        Debug.Log($"[DEBUG] Model.playerData.levels {Model.playerData.levels}.");
        Debug.Log($"[DEBUG] Model.playerData.levels.count {Model.playerData.levels.Count}.");
        if(Model.playerData.levels.Count>=EndGameMap.LAST_LEVEL)
        {
            mapEndGame.Open();
        }
        else
        {
            mapManager.Open();
        }
    }
}
