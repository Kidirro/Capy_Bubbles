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
