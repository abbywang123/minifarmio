using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject shopPanel;                    // 整體商店面板
    public Button openShopButton;                   // 開啟商店按鈕
    public Button buyTabButton;                     // 買入分頁按鈕
    public Button sellTabButton;                    // 賣出分頁按鈕

    [Header("ScrollView 面板")]
    public GameObject buyScrollView;                // Buy 的 ScrollView 整體物件
    public GameObject sellScrollView;               // Sell 的 ScrollView 整體物件
    public Transform buyContentParent;              // Buy ScrollView 中的 Content
    public Transform sellContentParent;             // Sell ScrollView 中的 Content

    [Header("Prefab")]
    public GameObject shopItemUIPrefab;             // 商品項目預製體

    [Header("玩家系統")]
    public PlayerWallet playerWallet;               // 玩家錢包
    public Inventory playerInventory;               // 玩家背包

    [Header("顯示金錢")]
    public TextMeshProUGUI playerMoneyText;

    private ShopItemInfo[] shopItems;

    void Start()
    {
        shopPanel.SetActive(false);

        openShopButton.onClick.AddListener(ToggleShopPanel);
        buyTabButton.onClick.AddListener(() => SwitchTab(true));
        sellTabButton.onClick.AddListener(() => SwitchTab(false));
    }

    void ToggleShopPanel()
    {
        bool isActive = !shopPanel.activeSelf;
        shopPanel.SetActive(isActive);

        if (isActive)
        {
            LoadShopItems();
            SwitchTab(true); // 預設顯示買入
            UpdateMoneyUI();
        }
    }

    void SwitchTab(bool showBuy)
    {
        buyScrollView.SetActive(showBuy);
        sellScrollView.SetActive(!showBuy);

        buyTabButton.interactable = !showBuy;
        sellTabButton.interactable = showBuy;
    }

    void LoadShopItems()
    {
        ClearChildren(buyContentParent);
        ClearChildren(sellContentParent);

        shopItems = Resources.LoadAll<ShopItemInfo>("ShopItems");

        foreach (var item in shopItems)
        {
            if (item.canBuy)
                CreateShopItemUI(item, buyContentParent, true);

            if (item.canSell)
                CreateShopItemUI(item, sellContentParent, false);
        }
    }

    void CreateShopItemUI(ShopItemInfo item, Transform parent, bool isBuy)
    {
        GameObject obj = Instantiate(shopItemUIPrefab, parent);
        ShopItemUI ui = obj.GetComponent<ShopItemUI>();

        string priceText = isBuy ? $"買：{item.buyPrice}" : $"賣：{item.sellPrice}";

        ui.Setup(
            item.itemName,
            item.icon,
            priceText,
            isBuy,
            !isBuy,
            () => {
                if (TryBuyItem(item)) UpdateMoneyUI();
            },
            () => {
                if (TrySellItem(item)) UpdateMoneyUI();
            }
        );
    }

    bool TryBuyItem(ShopItemInfo item)
    {
        if (!playerWallet.CanAfford(item.buyPrice)) return false;
        if (!playerInventory.Add(item.itemData, 1)) return false;

        playerWallet.Spend(item.buyPrice);
        return true;
    }

    bool TrySellItem(ShopItemInfo item)
    {
        if (!playerInventory.Remove(item.itemData, 1)) return false;

        playerWallet.Earn(item.sellPrice);
        return true;
    }

    void UpdateMoneyUI()
    {
        playerMoneyText.text = $"金錢：{playerWallet.CurrentMoney}";
    }

    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
