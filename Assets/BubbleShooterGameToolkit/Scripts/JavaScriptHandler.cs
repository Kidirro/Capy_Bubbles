using System;
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

    public static string GetToken()
    {
#if MEGAFON
        return GetTokenFromParametersOrCookies();
#elif BEELINE
        GetTokenFromParameters();
        return "";
#endif

    }
    
#if MEGAFON
    [DllImport("__Internal")]
    public static extern void Quit();

    [DllImport("__Internal")]
    public static extern string GetQuery();

    [DllImport("__Internal")]
    private static extern void CallConfirmPurchaseNew(string text);
    
    
    [DllImport("__Internal")]
    private static extern string GetTokenFromParametersOrCookies();

    public static void ConfirmPurchase(int price, string purchaseData, string description)
    {
        var data = new PurcaseDataNew
        {
            game_key = 1,
            price = price * 100,
            transactionId = Guid.NewGuid().ToString("N")[..20],
            data = purchaseData,
            title = "Покупка в игре «Приключения Лайфи»",
            subtitle = $"Стоимость: {price} сомони",
            description = description
        };

        string json = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        Debug.Log("new window " + json);
        CallConfirmPurchaseNew(json);
    }
     public static string isTestVersion()
    {
        return GetQuery();
    }

    [System.Serializable]
    private class PurcaseDataNew
    {
        public int game_key;
        public int price;
        public string transactionId;
        public string data;
        public string description;
        public string title;
        public string subtitle;
    }
#endif
}
