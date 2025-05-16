
#if MEGAFON
using System;
using System.Linq;
using System.Threading.Tasks;
using BubbleShooterGameToolkit.Scripts.CommonUI.Popups;
using BubbleShooterGameToolkit.Scripts.Settings;
using Newtonsoft.Json;
using UnityEngine.Events;
#endif
using UnityEngine;

public class MegafonShopTJS : MonoBehaviour
{
#if MEGAFON
    public static MegafonShopTJS instance;
    public static string query { get; private set; }

    private static UnityAction onBuyAction = null;
    private static UnityAction errorAction = null;
    private static TJSShopType waitingToConfirmSlot = TJSShopType.None;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
#if !UNITY_EDITOR && MEGAFON
            
        JavaScriptHandler.isTestVersion();
            
#else
        query = "2";
#endif
    }

    private void IsTest(string query)
    {
        MegafonShopTJS.query = query;
        Debug.Log("in game query is " + query);
    }

    private void SlotConfirmed(string json)
    {
        if (json == null || json == "") return;
        Debug.Log("Waiting slot is: " + waitingToConfirmSlot);
        if (waitingToConfirmSlot == TJSShopType.None) return;

        ExternConfirmationData data = JsonConvert.DeserializeObject<ExternConfirmationData>(json);
        Debug.Log("Unity get data: " + json);
        Debug.Log("Status code: " + data.status);
        Debug.Log("Data: " + data.data);
        switch (waitingToConfirmSlot)
        {
            case TJSShopType.BuyGold:
            case TJSShopType.BuyGems:
                _ = ProductConfirmed(data);
                break;
            default:
                break;
        }

        waitingToConfirmSlot = TJSShopType.None;
    }

    public static void BuyProduct(ItemPurchase data, UnityAction onBuyAction = null, UnityAction errorAction = null, TJSShopType type = TJSShopType.None)
    {
        if (type == TJSShopType.None) return;
        if (query.Length > 0)
        {
            waitingToConfirmSlot = type;
            string Route = "";
            switch (type)
            {
                case TJSShopType.BuyGold:
                    Route = "gold";
                    break;
                case TJSShopType.BuyGems:
                    Route = "gems";
                    break;
            }
            if (onBuyAction != null)
            {
                MegafonShopTJS.onBuyAction = onBuyAction;
            }
            if (errorAction != null)
            {
                MegafonShopTJS.errorAction = errorAction;
            }
            JavaScriptHandler.ConfirmPurchase(
                   int.Parse(data._price),
                   JsonConvert.SerializeObject(new TarificationData()
                   {
                       route = Route,
                       data = JsonConvert.SerializeObject(new ProductWithID()
                       {
                           id = data.id,
                       })
                   }),
                   "Деньги спишутся со счёта телефона"
               );
        }
    }

    private static async Task ProductConfirmed(ExternConfirmationData data)
    {
        if (data.status == 200)
        {
            await Model.UpdateData();
            //Нужен ли попап?
            onBuyAction.Invoke();
        }
        else if (data.status == 511 || data.status == 402)
        {
            BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<InfoPopup>().SetText(textsInfo: TextsInfo.NotPayment);

        }
        else if (data.status == 502)
        {
            BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<InfoPopup>().SetCustomText(data.data);
        }
        else if (data.status == 499)
        {
            BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<InfoPopup>().SetCustomText(data.data);
        }
        else
        {
            Debug.LogError(data.status);               
            BubbleShooterGameToolkit.Scripts.CommonUI.MenuManager.instance.ShowPopup<InfoPopup>().SetCustomText(data.data);
        }
    }


    [Serializable]
    private class ExternConfirmationData
    {
        public int status;
        public string data;
    }

    class TarificationShopData
    {
        public int index { get; set; }
        internal TarificationShopData(int indx)
        {
            index = indx;
        }
        public override string ToString()
        {
            return $"index={index}";
        }
    }
    public class ProductWithID
    {
        public string id { get; set; }
    }
    public class TarificationData
    {
        public string route { get; set; }
        public dynamic data;
    }
    public enum TJSShopType
    {
        None = -1,
        BuyGold = 0,
        BuyGems = 1
    }
#endif
}
