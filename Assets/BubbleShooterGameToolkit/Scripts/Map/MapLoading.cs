using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLoading : MonoBehaviour
{   
    [Space,SerializeField] private CanvasGroup loadingCanvas;
    [SerializeField] private Image loadingBar;
    
    public CanvasGroup LoadingCanvas => loadingCanvas;
    public Image LoadingBar => loadingBar;
}
