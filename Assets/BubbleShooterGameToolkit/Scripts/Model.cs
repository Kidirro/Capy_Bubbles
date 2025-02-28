using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BubbleShooterGameToolkit.Scripts.Data;
using BubbleShooterGameToolkit.Scripts.System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
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

    public static string backend = "https://bubbles.arcomm.ru/api/";
    private static string telegram = "https://t.me/Rozi_cat";
    void Start()
    {
        //_telegram.onClick.AddListener(OpenTelegram);
#if PLUGIN_YG_2
        UpdateData();
#else
        SendGetToken();
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
        playerData = new PlayerData(true);
    }

    private static async Task UpdateData()
    {
        playerData =
#if PLUGIN_YG_2
            GetSaveYG();
#else
            await GetSave();
#endif
        id = playerData.id;
        phone = playerData.phone;
        if (playerData.endGameFirstMapObjectsOpen.Length == 0)
            playerData.endGameFirstMapObjectsOpen = new bool[25];
        if (playerData.endGameSecondMapObjectsOpen.Length == 0)
            playerData.endGameSecondMapObjectsOpen = new bool[27];
        GameManager.instance.coins.Set(playerData.gold);
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
                return JsonUtility.FromJson<PlayerData>(request.downloadHandler.text.Replace(":\"", ":").Replace("\",", ",").Replace("\"}", "}"));


            default:
                Debug.Log("Error: " + request.responseCode + " Message: " + request.downloadHandler.error);
                return new PlayerData(false);

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

        var tempPlayerData = JsonUtility.FromJson<PlayerData>(YG2.saves.playerDataJson) ?? new PlayerData(false);
        return tempPlayerData;

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

    public static async Task BuyProduct(string id)
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
    public static int GetScore(){
        int result=0;
        for (int i = 0; i < playerData.endGameFirstMapObjectsOpen.Length; i++)
        {
            if(playerData.endGameFirstMapObjectsOpen[i])
            {
                result += Mathf.RoundToInt(GameManager.instance.endGameSetting.mapObjectCost[0].mapObjectCost[i]/10f);
            }
        }
        for (int i = 0; i<playerData.endGameSecondMapObjectsOpen.Length;i++){
            if(playerData.endGameSecondMapObjectsOpen[i])
            {
                result += Mathf.RoundToInt(GameManager.instance.endGameSetting.mapObjectCost[1].mapObjectCost[i]/10);
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
    public int counterLevel = 0;
    public string endGameFirstMapObjectsOpen;
    public string endGameSecondMapObjectsOpen;
    public int score=0;

    public PlayerSendData(PlayerData data)
    {
        gold = data.gold;
        hearts = data.hearts;
        counterLevel = data.counterLevel;


        endGameFirstMapObjectsOpen = "[";
        bool isZero = true;
        foreach (var obj in data.endGameFirstMapObjectsOpen)
        {
            isZero = false;
            endGameFirstMapObjectsOpen += obj.ToString().ToLower() + ",";
        }
        if (!isZero)
            endGameFirstMapObjectsOpen = endGameFirstMapObjectsOpen.Remove(endGameFirstMapObjectsOpen.Length - 1);
        endGameFirstMapObjectsOpen += "]";

        endGameSecondMapObjectsOpen = "[";
        bool isZero1 = true;
        foreach (var obj in data.endGameSecondMapObjectsOpen)
        {
            isZero1 = false;
            endGameSecondMapObjectsOpen += obj.ToString().ToLower() + ",";
        }
        if (!isZero1)
            endGameSecondMapObjectsOpen = endGameSecondMapObjectsOpen.Remove(endGameSecondMapObjectsOpen.Length - 1);
        endGameSecondMapObjectsOpen += "]";

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
    }
}
[Serializable]
public class PlayerData
{
    public int id = 0;
    public string phone;
    public int gold = 0;
    public int hearts = 0;
    public List<int> levels = new ();
    public int[] boosters = new int[4];
    public int counterLevel = 0;
    public bool[] endGameFirstMapObjectsOpen = new bool[25];
    public bool[] endGameSecondMapObjectsOpen = new bool[27];
    public int score=0;

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

        gold = GameManager.instance.coins.GetResource();
        hearts = GameManager.instance.life.GetResource();
        if (Model.playerData == null)
        {
            counterLevel = 0;
            endGameFirstMapObjectsOpen = new bool[25];
        }
        else
        {
            counterLevel = Model.playerData.counterLevel;
            endGameFirstMapObjectsOpen = Model.playerData.endGameFirstMapObjectsOpen;
        }
        endGameFirstMapObjectsOpen = Model.playerData.endGameFirstMapObjectsOpen;
        endGameSecondMapObjectsOpen = Model.playerData.endGameSecondMapObjectsOpen;
        score = Model.GetScore();
    }
    public PlayerData()
    {
        GameManager.instance.life.LoadPrefs();
    }
}
