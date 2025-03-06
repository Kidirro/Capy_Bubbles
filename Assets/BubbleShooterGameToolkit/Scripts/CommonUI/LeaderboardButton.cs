using System.Collections;
using System.Collections.Generic;
using BubbleShooterGameToolkit.Scripts.CommonUI;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class LeaderboardButton : MonoBehaviour
{
        [SerializeField]
        private Button spinButton;

        
        private void OnEnable()
        {
            spinButton.onClick.AddListener(OpenSpin);
        }

        private void OpenSpin()
        {
            MenuManager.instance.ShowPopup<Rating>();
        }
    
}
