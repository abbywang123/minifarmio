using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI 元件")]
    public Image itemIcon;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;

    public Button buyButton;
    public Button sellButton;

    public void Setup(string name, Sprite icon, string priceText, bool canBuy, bool canSell,
                      UnityEngine.Events.UnityAction onBuyClicked,
                      UnityEngine.Events.UnityAction onSellClicked)
    {
        itemNameText.text = name;
        itemIcon.sprite = icon;
        itemPriceText.text = priceText;

        buyButton.gameObject.SetActive(canBuy);
        sellButton.gameObject.SetActive(canSell);

        if (canBuy)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(onBuyClicked);
        }

        if (canSell)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(onSellClicked);
        }
    }
}
