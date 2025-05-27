using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button buySellButton;
    public TextMeshProUGUI buttonText;

    private ShopItemInfo item;
    private bool isBuyMode;

    public void Setup(ShopItemInfo data, bool buyMode)
    {
        item = data;
        isBuyMode = buyMode;

        iconImage.sprite = data.icon;
        nameText.text = data.itemName;

        if (isBuyMode)
        {
            priceText.text = $"價格: {data.buyPrice}";
            buttonText.text = "購買";
            buySellButton.interactable = data.canBuy && PlayerWallet.Instance.CanAfford(data.buyPrice);
        }
        else
        {
            priceText.text = $"價格: {data.sellPrice}";
            buttonText.text = "賣出";
            buySellButton.interactable = data.canSell; // 可依擁有數量進一步判斷
        }

        buySellButton.onClick.RemoveAllListeners();
        buySellButton.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (isBuyMode)
        {
            if (PlayerWallet.Instance.Spend(item.buyPrice))
            {
                Debug.Log($"🛒 購買了 {item.itemName}");
                // 這裡可加到玩家的道具庫存
            }
            else
            {
                Debug.Log("❌ 錢不夠，無法購買！");
            }
        }
        else
        {
            PlayerWallet.Instance.Earn(item.sellPrice);
            Debug.Log($"💰 賣出了 {item.itemName}");
            // 這裡應從玩家庫存移除
        }

        // 更新按鈕狀態（選擇性）
        Setup(item, isBuyMode);
    }
}
