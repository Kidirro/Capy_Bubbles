using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BubbleShooterGameToolkit.Scripts.System;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityAsyncExtensions = Cysharp.Threading.Tasks.UnityAsyncExtensions;
#if YandexGamesPlatfom_yg
using YG;
#endif

public class Model : MonoBehaviour
{
    [SerializeField] private Button privacyOK;
    [SerializeField] private Button _telegram;
    [SerializeField] private Sprite _megafonQuit;
    [SerializeField] private GameObject privacy;

    [SerializeField] private GameObject errorPopup;
    [SerializeField] private Button acceptErrorBtn;
    
    
    private static GameObject errorPopupInstance;
    private static Button acceptErrorBtnInstance;
    private static string token;
    public static PlayerData playerData;
    public static int id;
    public static string phone;
#if MEGAFON
    public static string backend = "https://tj-capybubbles.rozicat.com/api/";
#else
    public static string backend = "https://bubbles.arcomm.ru/api/";
    private static string telegram = "https://t.me/Rozi_cat";
#endif
    void Start()
    {
        errorPopupInstance = errorPopup;
        acceptErrorBtnInstance = acceptErrorBtn;
#if MEGAFON
        _telegram.transform.GetChild(0).GetComponent<Image>().gameObject.SetActive(false);
        _telegram.GetComponent<Image>().sprite = _megafonQuit;
        _telegram.onClick.AddListener(JavaScriptHandler.Quit);
#else
        _telegram.onClick.AddListener(OpenTelegram);
#endif
#if UNITY_EDITOR
        //Token("3fdf1266a04f9cf495e106a297a69d5307a38281");
        Token("3fdf1266a04f9cf495e106a297a69d5307a38281");
#else
        SendGetToken();
#endif
#if MEGAFON
#elif BEELINE
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
    public async void Token(string token) 
    {
        Model.token = token;
        await UpdateData();
    }

    public void OpenTelegram()
    {

#if BEELINE && !MEGAFON
        JavaScriptHandler.OpenURLInSameTab(telegram);
#endif
    }

    private async void SendGetToken()
    {
#if MEGAFON
        Model.token = JavaScriptHandler.GetTokenFromParametersOrCookies();
        await UpdateData();
#elif BEELINE
        JavaScriptHandler.GetToken();
#endif

    }

    // public static async void TestToken()
    // {
    //     /*if (PlayerPrefs.HasKey("CatState"))
    //     {
    //         return JsonConvert.DeserializeObject<CatState>(PlayerPrefs.GetString("CatState"));
    //     }*/
    //     string jsonBody = "{\"status\":1,  \"transaction_id\": \"test\"}";

    //     var request = new UnityWebRequest("https://tj-capybubbles.rozicat.com/api/transactions/change", "POST");
    //     byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonBody);

    //     request.uploadHandler = new UploadHandlerRaw(jsonToSend);
    //     request.downloadHandler = new DownloadHandlerBuffer();
    //     request.SetRequestHeader("Content-Type", "application/json");
    //     request.SetRequestHeader("authorization", "Bearer 5ea39e867aab868f7eeccafa749ff80b98fd4837b60baac0993626cdf0461f75");

    //     await request.SendWebRequest();
    //     Debug.Log(request.responseCode + " code");
    //     Debug.Log("Data "+request.downloadHandler.text);
    // }
    public static async void TestToken()
    {
        /*if (PlayerPrefs.HasKey("CatState"))
        {
            return JsonConvert.DeserializeObject<CatState>(PlayerPrefs.GetString("CatState"));
        }*/
        string jsonBody = "{\"gamekey\":\"2\", \"price\": \"1\", \"data\": \"{\\\"data\\\":{\\\"product_id\\\":2240},\\\"route\\\":\\\"shop\\\"}\", \"transaction_id\": \"test\", \"megafon_user_id\": \"19688049_19831480\"}";

        var request = new UnityWebRequest("https://tj-capybubbles.rozicat.com/token/megafon/register/2", "POST");
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("authorization", "Bearer 5ea39e867aab868f7eeccafa749ff80b98fd4837b60baac0993626cdf0461f75");

        await request.SendWebRequest();
        Debug.Log(request.responseCode + " code");
        Debug.Log("Data "+request.downloadHandler.text);
    }


    private static void GetData()
    {
        playerData = new PlayerData(true);
    }

    public static async Task UpdateData()
    {
        playerData = await GetSave();
        id = playerData.id;
        phone = playerData.phone;

        if (playerData.endGameMapObjects == null || playerData.endGameMapObjects.Count == 0)
        {
            playerData.endGameMapObjects = new Dictionary<int, List<bool>>();
            for (int i = 0; i < GameManager.instance.endGameSetting.mapObjectCost.Length; i++)
            {
                playerData.endGameMapObjects.Add(i, new bool [
                    GameManager.instance.endGameSetting.mapObjectCost[i].mapObjectCost.Length].ToList());
            }
        }
        
        GameManager.instance.coins.Set(playerData.gold);
        GameManager.instance.gem.Set(playerData.gems);
        for (int i = 0; i < GameManager.instance.boosters.Length; i++)
        {
            GameManager.instance.boosters[i].Set(i < playerData.boosters.Length ? playerData.boosters[i] : 0);
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
#if YandexGamesPlatfom_yg
        return YG2.saves.jsonSave == "" ? new PlayerData() : JsonUtility.FromJson<PlayerData>(YG2.saves.jsonSave);

#else
        var request = UnityWebRequest.Get(backend + "user");
        request.SetRequestHeader("accessToken", token);
        request.SetRequestHeader("Content-Type", ""); // Очищаем
        await request.SendWebRequest();
        switch (request.responseCode)
        { 
            case 200:
                Debug.Log("Save: " + request.downloadHandler.text);
                
                var rawData = JsonUtility.FromJson<SaveData>(request.downloadHandler.text);
                
               var tmpData = JsonUtility.FromJson<PlayerData>(request.downloadHandler.text.Replace("\"[", "[").Replace("]\"", "]"));

                if (rawData.endGameMapObjects != "[]")
                {
                    tmpData.endGameMapObjects =
                        JsonConvert.DeserializeObject<Dictionary<int, List<bool>>>(rawData.endGameMapObjects);
                }
                else
                {
                    
                    tmpData.endGameMapObjects = new Dictionary<int, List<bool>>();
                    if (rawData.endGameFirstMapObjectsOpen != null && rawData.endGameFirstMapObjectsOpen != "[]")
                    {
                        tmpData.endGameMapObjects[0] = JsonConvert.DeserializeObject<List<bool>>(rawData.endGameFirstMapObjectsOpen);
                    }

                    if (rawData.endGameSecondMapObjectsOpen != null && rawData.endGameSecondMapObjectsOpen != "[]")
                    {
                        tmpData.endGameMapObjects[1] = JsonConvert.DeserializeObject<List<bool>>(rawData.endGameSecondMapObjectsOpen);
                    }
                }

                tmpData.upgrdadeData = rawData.upgradeData != "[]" ? JsonConvert.DeserializeObject<Dictionary<string,int>>(rawData.upgradeData) : new Dictionary<string,int>();
                
                if (errorPopupInstance != null)
                    errorPopupInstance.SetActive(false);
                return tmpData;


            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.text);
                errorPopupInstance?.SetActive(true);
                acceptErrorBtnInstance.interactable = true;
                
                await UnityAsyncExtensions.OnClickAsync(acceptErrorBtnInstance);
                //wait btn
                Debug.Log("CLick!");
                acceptErrorBtnInstance.interactable = false;
                return await GetSave();

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
    
    public static async Task<bool> BuyProduct(string id)
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
                return true;
            case 402:

                BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<BubbleShooterGameToolkit.Scripts.CommonUI.Popups.InfoPopup>().SetText(textsInfo: BubbleShooterGameToolkit.Scripts.CommonUI.Popups.TextsInfo.NotPayment);
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error + "Detail: " + request.downloadHandler.text);
                return false;
            case 403:
            case 504:
            case 408:
                BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<BubbleShooterGameToolkit.Scripts.CommonUI.Popups.InfoPopup>().SetText(textsInfo: BubbleShooterGameToolkit.Scripts.CommonUI.Popups.TextsInfo.Timeout);
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error + "Detail: " + request.downloadHandler.text);
                return false;

            default:
                BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<BubbleShooterGameToolkit.Scripts.CommonUI.Popups.InfoPopup>().SetText(textsInfo: BubbleShooterGameToolkit.Scripts.CommonUI.Popups.TextsInfo.ErrorPayment);
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error + "Detail: " + request.downloadHandler.text);
                return false;
        }

    }

    public static int GetScore()
    {
        int result = 0;

        for (int i = 0; i < playerData.endGameMapObjects.Count; i++)
        {
            for (int j = 0; j < playerData.endGameMapObjects[i].Count; j++)
            {
                if (playerData.endGameMapObjects[i][j])
                {
                    result += Mathf.RoundToInt(GameManager.instance.endGameSetting.mapObjectCost[i].mapObjectCost[j] / 10f); 
                }
            }
        }
        
        return result;
    }
    public static async Task SetSave()
    {
        /*if (PlayerPrefs.HasKey("CatState"))
        {
            return JsonConvert.DeserializeObject<CatState>(PlayerPrefs.GetString("CatState"));
        }*/
        Debug.Log("SetSave");

#if YandexGamesPlatfom_yg
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

    public static string GetToken()
    {
        return token;
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
    public int gems;
}
[Serializable]
public class PlayerSendData
{

    public int gold = 0;
    public int hearts = 0;
    public int gems = 0;
    public string levels;
    public string boosters;
    public int counterLevel = 0;
    public int score = 0;
    public string endGameMapObjects;
    public string upgradeData;

    public PlayerSendData(PlayerData data)
    {
        gold = data.gold;
        gems = data.gems;
        hearts = data.hearts;
        counterLevel = data.counterLevel;

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
        score = Model.GetScore();
        endGameMapObjects = JsonConvert.SerializeObject(data.endGameMapObjects);
        upgradeData = JsonConvert.SerializeObject(data.upgrdadeData);
        Debug.Log(JsonConvert.SerializeObject(data.upgrdadeData));
    }
}

[Serializable]
public class SaveData
{
    public int id;
    public string endGameMapObjects;
    public string endGameFirstMapObjectsOpen;
    public string endGameSecondMapObjectsOpen;
    
    public string upgradeData;
}

[Serializable]
public class PlayerData
{
    public int id = 0;
    public string phone;
    public int gold = 0;
    public int hearts = 0;
    public int gems = 0;
    public List<int> levels = new List<int>();
    public int[] boosters = new int[4];
    public int counterLevel = 0;
    public Dictionary<int, List<bool>> endGameMapObjects = new Dictionary<int, List<bool>>();
    public Dictionary<string, int> upgrdadeData = new Dictionary<string, int>();
    public int score = 0;

    public PlayerData(bool toserver)
    {
        levels = new List<int>();
        for (int i = 0; i < PlayerPrefs.GetInt("Level", 1); i++)
        {
            int value = PlayerPrefs.GetInt("LevelScore" + (i + 1), 0);
            if (value > 0)
                levels.Add(value);
        }
        boosters = new int[4];
        counterLevel = Model.playerData.counterLevel;
        GameManager.instance.coins.LoadPrefs();
        GameManager.instance.life.LoadPrefs();
        for (int i = 0; i < GameManager.instance.boosters.Length; i++)
        {
            GameManager.instance.boosters[i].LoadPrefs();
            boosters[i] = GameManager.instance.boosters[i].GetResource();
        }

        phone = Model.playerData.phone;
        gold = GameManager.instance.coins.GetResource();
        gems = GameManager.instance.gem.GetResource();
        hearts = GameManager.instance.life.GetResource();
        endGameMapObjects = Model.playerData.endGameMapObjects;
        score = Model.GetScore();
        upgrdadeData = Model.playerData.upgrdadeData;
    }
    public PlayerData()
    {
        GameManager.instance.life.LoadPrefs();
    }
}
