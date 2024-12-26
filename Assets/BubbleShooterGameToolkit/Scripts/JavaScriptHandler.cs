using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class JavaScriptHandler : MonoBehaviour
{
    #if BEELINE

    [DllImport("__Internal")]
    public static extern void GetTokenFromParameters();

    [DllImport("__Internal")]
    public static extern void OpenURLInSameTab(string url);
    [DllImport("__Internal")]
    public static extern void PauseSound();
    [DllImport("__Internal")]
    public static extern void ResumeSound();
    #endif
}
