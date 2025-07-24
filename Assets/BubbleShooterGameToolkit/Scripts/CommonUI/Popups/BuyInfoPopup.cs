using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BubbleShooterGameToolkit.Scripts.CommonUI.Popups
{
    public class BuyInfoPopup : Popup
    {
        [SerializeField] private TMP_Text _info;
        [SerializeField] private TMP_Text _price;
        [SerializeField] private Button _confirm;
        public void SetText(string prize, string price, bool isCoins, UnityAction onConfirm)
        {
            _confirm.onClick.RemoveAllListeners();
            _confirm.onClick.AddListener(Close);
            _confirm.onClick.AddListener(onConfirm);
            var key = isCoins ? "COINS" : "GEMS";
            _info.text = /*LocalizeStorage.GetText("YOU_BUY", LocalizationChanger.language)+" "+*/prize + " " + LocalizeStorage.GetText(key, LocalizationChanger.language);
#if MEGAFON
            _price.text = LocalizeStorage.GetText("PRICE", LocalizationChanger.language) + " " + price + " сомони";
#elif BEELINE
            _price.text = LocalizeStorage.GetText("PRICE", LocalizationChanger.language) + " " + price + " " + LocalizeStorage.GetText("RUB", LocalizationChanger.language);
#endif
        }
    }
}