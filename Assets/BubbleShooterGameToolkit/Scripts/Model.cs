using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BubbleShooterGameToolkit.Scripts.System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using YG;

public class Model : MonoBehaviour
{
    [SerializeField] private Button privacyOK;
    [SerializeField] private Button _telegram;
    [SerializeField] private GameObject privacy;
    private static string token;
    public static PlayerData playerData;

    private static string backend = "https://bubbles.arcomm.ru/api/";
    private static string telegram = "https://t.me/Rozi_cat";
    void Start()
    {
        _telegram.onClick.AddListener(OpenTelegram);
#if UNITY_EDITOR
        Token("3fdf1266a04f9cf495e106a297a69d5307a38281");
#else
        SendGetToken();
#endif
#if BEELINE
        if (PlayerPrefs.GetFloat("firstOpen", 0) == 0)
        {
            privacy.SetActive(true);
            privacyOK.onClick.AddListener(() =>
            {
                PlayerPrefs.SetFloat("firstOpen", 1);
                privacy.SetActive(false);
            });
        }

#else
#endif
    }
    public async void Token(string token) // this calling from webview
    {
        Model.token = token;
        await UpdateData();
    }

    public void OpenTelegram()
    {
        
#if BEELINE
        JavaScriptHandler.OpenURLInSameTab(telegram);
#endif
    }

    private void SendGetToken()
    {
#if  BEELINE
        JavaScriptHandler.GetTokenFromParameters();
#endif

    }



    private static void GetData()
    {
        playerData = new PlayerData();
    }

    private static async Task UpdateData()
    {
        playerData = await GetSave();
        GameManager.instance.coins.Set(playerData.gold);
        for (int i = 0; i < GameManager.instance.boosters.Length; i++)
        {
            GameManager.instance.boosters[i].Set(i<playerData.boosters.Length?playerData.boosters[i]:0);
        }

        if (PlayerPrefs.GetInt("Level", 1) < playerData.levels.Count + 1)
            PlayerPrefs.SetInt("Level", playerData.levels.Count + 1);

        for (int i = 1; i < playerData.levels.Count + 1; i++)
        {
            PlayerPrefs.SetInt("LevelScore" + i, playerData.levels[i - 1]);
        }
        PlayerPrefs.Save();

    }

    private static async Task<PlayerData> GetSave()
    {
        /*if (PlayerPrefs.HasKey("CatState"))
        {
            return JsonConvert.DeserializeObject<CatState>(PlayerPrefs.GetString("CatState"));
        }*/
#if UNITY_WEBGL
        return YG2.saves.jsonSave == "" ? new PlayerData() : JsonUtility.FromJson<PlayerData>(YG2.saves.jsonSave);

#else
        var request = UnityWebRequest.Get(backend + "user");
        request.SetRequestHeader("accessToken", token);
        await request.SendWebRequest();
        switch (request.responseCode)
        {
            case 200:
                Debug.Log("Save: " + request.downloadHandler.text);
                return JsonUtility.FromJson<PlayerData>(request.downloadHandler.text.Replace(":\"", ":").Replace("\",", ",").Replace("\"}", "}"));


            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                return new PlayerData();

        }
#endif
    }

    public static async Task<Shop> GetShopProduts()
    {
        /*if (PlayerPrefs.HasKey("CatState"))
        {
            return JsonConvert.DeserializeObject<CatState>(PlayerPrefs.GetString("CatState"));
        }*/
        var request = UnityWebRequest.Get(backend + "product");
        await request.SendWebRequest();
        switch (request.responseCode)
        {
            case 200:
                Debug.Log("Products: " + request.downloadHandler.text);
                return JsonUtility.FromJson<Shop>(request.downloadHandler.text);

            case 500:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                return new Shop();

            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                return null;
        }

    }

    public static async Task BuyProduct(string id)
    {
        /*if (PlayerPrefs.HasKey("CatState"))
        {
            return JsonConvert.DeserializeObject<CatState>(PlayerPrefs.GetString("CatState"));
        }*/
        var request = UnityWebRequest.Post(backend + $"user/buy?productId={id}", "", "application/json");
        request.SetRequestHeader("accessToken", token);
        await request.SendWebRequest();
        switch (request.responseCode)
        {
            case 200:
                await UpdateData();
                Debug.Log("Product Buing: " + request.downloadHandler.text);
                return;
            case 402:

                BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<BubbleShooterGameToolkit.Scripts.CommonUI.Popups.InfoPopup>().SetText(textsInfo: BubbleShooterGameToolkit.Scripts.CommonUI.Popups.TextsInfo.NotPayment);
                return;
            case 403:
            case 504:
            case 408:
                BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<BubbleShooterGameToolkit.Scripts.CommonUI.Popups.InfoPopup>().SetText(textsInfo: BubbleShooterGameToolkit.Scripts.CommonUI.Popups.TextsInfo.Timeout);
                return;

            default:
                BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<BubbleShooterGameToolkit.Scripts.CommonUI.Popups.InfoPopup>().SetText(textsInfo: BubbleShooterGameToolkit.Scripts.CommonUI.Popups.TextsInfo.ErrorPayment);
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error + "Detail: " + request.downloadHandler.text);
                return;
        }

    }

    public static async Task SetSave()
    {
        /*if (PlayerPrefs.HasKey("CatState"))
        {
            return JsonConvert.DeserializeObject<CatState>(PlayerPrefs.GetString("CatState"));
        }*/
        Debug.Log("SetSave");
        
#if UNITY_WEBGL
        YG2.saves.jsonSave = JsonUtility.ToJson(new PlayerSendData(playerData));
        YG2.SaveProgress();
#else
        
        
        GetData();

        var request = UnityWebRequest.Put(backend + "user", JsonUtility.ToJson(new PlayerSendData(playerData)));
        request.SetRequestHeader("accessToken", token);
        request.uploadHandler.contentType = "application/json";
        await request.SendWebRequest();
        switch (request.responseCode)
        {
            case 200:
                return;// JsonUtility.FromJson<PlayerData>(request.downloadHandler.text);
            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error + "Detail: " + request.downloadHandler.text);
                return;// new PlayerData();
        }
#endif

    }
}

[Serializable]
public class Shop
{
    public ProductShop[] products;
}
[Serializable]
public class ProductShop
{
    public string id;
    public string price;
    public int gold;
}
[Serializable]
public class PlayerSendData
{

    public int gold = 0;
    public int hearts = 0;
    public string levels;
    public string boosters;
    public PlayerSendData(PlayerData data)
    {
        gold = data.gold;
        hearts = data.hearts;
        levels = "[";
        foreach (var level in data.levels)
        {
            levels += level + ",";
        }
        if (data.levels.Count > 0)
            levels = levels.Remove(levels.Length - 1);
        levels += "]";

        boosters = "[";
        foreach (var level in data.boosters)
        {
            boosters += level + ",";
        }
        boosters = boosters.Remove(boosters.Length - 1);
        boosters += "]";
    }
}
[Serializable]
public class PlayerData
{
    public int gold = 0;
    public int hearts = 0;
    public List<int> levels;
    public int[] boosters = new int[4];

    public PlayerData()
    {
        levels = new List<int>();
        for (int i = 0; i < PlayerPrefs.GetInt("Level", 1); i++)
        {
            int value = PlayerPrefs.GetInt("LevelScore" + (i + 1), 0);
            if (value > 0)
                levels.Add(value);
        }
        boosters = new int[4];
        GameManager.instance.coins.LoadPrefs();
        GameManager.instance.life.LoadPrefs();
        for (int i = 0; i < GameManager.instance.boosters.Length; i++)
        {
            GameManager.instance.boosters[i].LoadPrefs();
            boosters[i] = GameManager.instance.boosters[i].GetResource();
        }

        gold = GameManager.instance.coins.GetResource();
        hearts = GameManager.instance.life.GetResource();
    }
}
