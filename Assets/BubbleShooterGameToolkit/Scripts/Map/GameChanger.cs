using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.Map;
using UnityEngine;

public class GameChanger : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private EndGameMap mapEndGame;

    void Awake()
    {
        if(Model.playerData.levels.Count>=EndGameMap.LAST_LEVEL)
        {
            var mapend = Instantiate<EndGameMap>(mapEndGame,this.transform);
            mapend.Open();
        }
        else
        {
            var map = Instantiate<MapManager>(mapManager,this.transform);
            map.Open();
        }
    }
}
