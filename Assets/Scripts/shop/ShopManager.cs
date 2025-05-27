using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject shopPanel;             // 商店面板
    public Button shopButton;                // 開關商店按鈕
    public Transform contentParent;          // 商店物品列表父物件（ScrollView Content）
    public GameObject shopItemUIPrefab;     // 商店物品 UI 預置物件
    public Text playerMoneyText;             // 玩家金錢顯示

    [Header("玩家背包與錢包")]
    public Inventory playerInventory;        // 玩家背包
    private PlayerWallet playerWallet;       // 玩家錢包 (從單例拿)

    void Start()
    {
        playerWallet = PlayerWallet.Instance;

        shopPanel.SetActive(false);          // 預設隱藏商店
        shopButton.onClick.AddListener(() => shopPanel.SetActive(!shopPanel.activeSelf));

        LoadShopItems();
        UpdateMoneyUI();

        // 訂閱錢包變動事件，保持 UI 即時更新
        if (playerWallet != null)
            playerWallet.OnMoneyChanged += OnMoneyChanged;
    }

    void OnDestroy()
    {
        if (playerWallet != null)
            playerWallet.OnMoneyChanged -= OnMoneyChanged;
    }

    void OnMoneyChanged(int newAmount)
    {
        UpdateMoneyUI();
    }

    // 載入商店物品列表
    void LoadShopItems()
    {
        // 清空舊 UI
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        ShopItemInfo[] items = Resources.LoadAll<ShopItemInfo>("ShopItems");

        foreach (var item in items)
        {
            GameObject obj = Instantiate(shopItemUIPrefab, contentParent);

            // 設定 UI 元件
            obj.transform.Find("ItemNameText").GetComponent<Text>().text = item.itemName;
            obj.transform.Find("ItemPriceText").GetComponent<Text>().text = $"💰{item.buyPrice}/{item.sellPrice}";
            obj.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.icon;

            Button buyBtn = obj.transform.Find("BuyButton").GetComponent<Button>();
            Button sellBtn = obj.transform.Find("SellButton").GetComponent<Button>();

            buyBtn.interactable = item.canBuy;
            sellBtn.interactable = item.canSell;

            // 加入事件 (閉包小心，使用局部變數)
            ShopItemInfo capturedItem = item;

            buyBtn.onClick.AddListener(() =>
            {
                if (TryBuyItem(capturedItem))
                    UpdateMoneyUI();
            });

            sellBtn.onClick.AddListener(() =>
            {
                if (TrySellItem(capturedItem))
                    UpdateMoneyUI();
            });
        }
    }

    // 嘗試購買物品
    bool TryBuyItem(ShopItemInfo item)
    {
        if (item.itemData == null)
        {
            Debug.LogError("商店物品未綁定 ItemData");
            return false;
        }

        if (!playerWallet.CanAfford(item.buyPrice))
        {
            Debug.Log("錢不夠購買此物品");
            return false;
        }

        if (playerInventory.Add(item.itemData, 1))
        {
            playerWallet.Spend(item.buyPrice);
            Debug.Log($"購買成功：{item.itemName}");
            return true;
        }
        else
        {
            Debug.Log("背包已滿，無法購買");
            return false;
        }
    }

    // 嘗試賣出物品
    bool TrySellItem(ShopItemInfo item)
    {
        if (item.itemData == null)
        {
            Debug.LogError("商店物品未綁定 ItemData");
            return false;
        }

        if (playerInventory.Remove(item.itemData, 1))
        {
            playerWallet.Earn(item.sellPrice);
            Debug.Log($"賣出成功：{item.itemName}");
            return true;
        }
        else
        {
            Debug.Log("背包內沒有該物品");
            return false;
        }
    }

    // 更新 UI 金錢顯示
    void UpdateMoneyUI()
    {
        playerMoneyText.text = $"金錢：{playerWallet.CurrentMoney}";
    }
}
