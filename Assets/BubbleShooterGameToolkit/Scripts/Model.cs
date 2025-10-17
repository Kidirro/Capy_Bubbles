using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BubbleShooterGameToolkit.Scripts.Data;
using BubbleShooterGameToolkit.Scripts.System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityAsyncExtensions = Cysharp.Threading.Tasks.UnityAsyncExtensions;
using YG.Insides;
#if PLUGIN_YG_2
using Newtonsoft.Json;
using YG;
#endif

public class Model : MonoBehaviour
{
    //[SerializeField] private Button _telegram;
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
        //_telegram.onClick.AddListener(OpenTelegram);
#if PLUGIN_YG_2
        UpdateData();
#else
        SendGetToken();
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

    private static void GetData()
    {
        playerData = new PlayerData(true);
    }

    public static async Task UpdateData()
    {
        playerData =
#if PLUGIN_YG_2
            GetSaveYG();
#else
            await GetSave();
#endif
        Debug.Log($"Player Data {playerData == null}");
        id = playerData.id;
        phone = playerData.phone;

        if (playerData.endGameMapObjects == null || playerData.endGameMapObjects.Count == 0)
        {
            playerData.endGameMapObjects = new Dictionary<int, List<bool>>();
            for (int i = 0; i < GameManager.instance.endGameSetting.mapCost.Count; i++)
            {
                playerData.endGameMapObjects.Add(i, new bool [
                    GameManager.instance.endGameSetting.mapCost[i].Data.Count].ToList());
            }
        }
        
        GameManager.instance.coins.Set(playerData.gold);
        GameManager.instance.gem.Set(playerData.gems);
        for (int i = 0; i < GameManager.instance.boosters.Length; i++)
        {
            GameManager.instance.boosters[i].Set(i < playerData.boosters.Length ? playerData.boosters[i] : 0);
        }

#if PLUGIN_YG_2
        for (int i = 1; i < playerData.levels.Count + 1; i++)
        {
            GameDataManager.instance.SaveLevel(i, playerData.levels[i - 1]);
        }
#else

        if (PlayerPrefs.GetInt("Level", 1) < playerData.levels.Count + 1)
            PlayerPrefs.SetInt("Level", playerData.levels.Count + 1);
        for (int i = 1; i < playerData.levels.Count + 1; i++)
        {
            PlayerPrefs.SetInt("LevelScore" + i, playerData.levels[i - 1]);
        }
        PlayerPrefs.Save();
#endif
    }

    private static async Task<PlayerData> GetSave()
    {
        /*if (PlayerPrefs.HasKey("CatState"))
        {
            return JsonConvert.DeserializeObject<CatState>(PlayerPrefs.GetString("CatState"));
        }*/
#if BEELINE
        var request = UnityWebRequest.Get(backend + "user");
        request.SetRequestHeader("accessToken", token);
        var operation =  request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

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

                errorPopupInstance.SetActive(false);
                return tmpData;


            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.text);
                errorPopupInstance.SetActive(true);
                acceptErrorBtnInstance.interactable = true;
                
                await UnityAsyncExtensions.OnClickAsync(acceptErrorBtnInstance);
                //wait btn
                Debug.Log("CLick!");
                acceptErrorBtnInstance.interactable = false;
                return await GetSave();

        }
#endif
        return new PlayerData();
    }

    private static PlayerData GetSaveYG()
    {
        if (YG2.player.id != PlayerPrefs.GetString("CurrentPlayer"))
        {
            PlayerPrefs.DeleteAll();
        }

        Debug.Log($" save {YG2.saves.playerDataJson == ""}");
        return YG2.saves.playerDataJson == "" ? new PlayerData(false) : JsonUtility.FromJson<PlayerData>(YG2.saves.playerDataJson);
    }
    public static async Task<Shop> GetShopProduts()
    {
        /*if (PlayerPrefs.HasKey("CatState"))
        {
            return JsonConvert.DeserializeObject<CatState>(PlayerPrefs.GetString("CatState"));
        }*/
#if PLUGIN_YG_2
       var shop = new Shop();
       
       var listProducts = new List<ProductShop>();

       for (int i = 0; i < YG2.purchases.Length; i++)
       {
           var productShop = new ProductShop();
           productShop.id = YG2.purchases[i].id;
           productShop.price = YG2.purchases[i].price;
           productShop.gold =int.Parse(YG2.purchases[i].id.Split("_")[^1]);
           
           listProducts.Add(productShop);
       }
    
       shop.products = listProducts.ToArray();
       return shop;

#else
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
#endif
    }
    
    public static async Task<bool> BuyProduct(string id)
    {
        /*if (PlayerPrefs.HasKey("CatState"))
        {
            return JsonConvert.DeserializeObject<CatState>(PlayerPrefs.GetString("CatState"));
        }*/
        var request = UnityWebRequest.Post(backend + $"user/buy?productId={id}", "", "application/json");
        request.SetRequestHeader("accessToken", token);
        var operation = request.SendWebRequest();
        
        while (!operation.isDone)
        {
            await Task.Yield();
        }
        
        switch (request.responseCode)
        {
            case 200:
                await UpdateData();
                Debug.Log("Product Buing: " + request.downloadHandler.text);
                return true;
            case 402:

                BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<BubbleShooterGameToolkit.Scripts.CommonUI.Popups.InfoPopup>().SetText(textsInfo: BubbleShooterGameToolkit.Scripts.CommonUI.Popups.TextsInfo.NotPayment);
                return false;
            case 403:
            case 504:
            case 408:
                BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<BubbleShooterGameToolkit.Scripts.CommonUI.Popups.InfoPopup>().SetText(textsInfo: BubbleShooterGameToolkit.Scripts.CommonUI.Popups.TextsInfo.Timeout);
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
                    var cost = GameManager.instance.endGameSetting.mapCost[i].Data[j];
                    if (cost.CurrencyType == CurrencyType.Coin)
                    {
                        result += Mathf.RoundToInt(cost.Value / 10f);
                    }
                    else
                    {
                        result += Mathf.RoundToInt(cost.Value * 4f);
                    }
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
        GetData();
        
#if PLUGIN_YG_2
        YG2.saves.playerDataJson = JsonUtility.ToJson(playerData);
        PlayerPrefs.SetString("CurrentPlayer", YG2.player.id);
        YG2.SaveProgress();
#else
        Debug.Log(JsonUtility.ToJson(new PlayerSendData(playerData)));
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
        
        Debug.Log(JsonConvert.SerializeObject(data.endGameMapObjects));
    }
}

[Serializable]
public class SaveData
{
    public int id;
    public string endGameMapObjects;
    public string endGameFirstMapObjectsOpen;
    public string endGameSecondMapObjectsOpen;
}

[Serializable]
public class PlayerData
{
    public int id = 0;
    public string phone;
    public int gold = 0;
    public int hearts = 0;
    public List<int> levels = new ();
    public int gems = 0;
    public int[] boosters = new int[4];
    public int counterLevel = 0;
    public Dictionary<int, List<bool>> endGameMapObjects = new Dictionary<int, List<bool>>();
    public int score = 0;

    public PlayerData(bool toserver)
    {
        levels = new List<int>();
#if PLUGIN_YG_2
        levels = JsonConvert.DeserializeObject(YG2.saves.levelData, typeof(List<int>)) as List<int> ?? new List<int>(){};
#else        
        for (int i = 0; i < PlayerPrefs.GetInt("Level", 1); i++)
        {
            int value = PlayerPrefs.GetInt("LevelScore" + (i + 1), 0);
            if (value > 0)
                levels.Add(value);
        }
#endif
        
        boosters = new int[4];
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
    }
    public PlayerData()
    {
        GameManager.instance.life.LoadPrefs();
    }
}
