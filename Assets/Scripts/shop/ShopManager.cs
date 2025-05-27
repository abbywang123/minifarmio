using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject shopPanel;
    public Button openShopButton;
    public Button buyTabButton;
    public Button sellTabButton;

    public Transform buyContentParent;
    public Transform sellContentParent;
    public GameObject shopItemUIPrefab;
    public TextMeshProUGUI playerMoneyText;

    [Header("遊戲系統")]
    public Inventory playerInventory;

    private ShopItemInfo[] shopItems;

    void Start()
    {
        shopPanel.SetActive(false);

        openShopButton.onClick.AddListener(ToggleShopPanel);
        buyTabButton.onClick.AddListener(() => SwitchTab(true));
        sellTabButton.onClick.AddListener(() => SwitchTab(false));

        // 訂閱金錢變動事件
        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.OnMoneyChanged += UpdateMoneyUI;

        UpdateMoneyUI(PlayerWallet.Instance?.CurrentMoney ?? 0);
    }

    void OnDestroy()
    {
        if (PlayerWallet.Instance != null)
            PlayerWallet.Instance.OnMoneyChanged -= UpdateMoneyUI;
    }

    void ToggleShopPanel()
    {
        bool isActive = !shopPanel.activeSelf;
        shopPanel.SetActive(isActive);

        if (isActive)
        {
            LoadShopItems();
            SwitchTab(true); // 預設顯示買入頁
        }
    }

    void SwitchTab(bool showBuy)
    {
        buyContentParent.gameObject.SetActive(showBuy);
        sellContentParent.gameObject.SetActive(!showBuy);

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

        string priceText = isBuy ? $"買: {item.buyPrice}" : $"賣: {item.sellPrice}";

        ui.Setup(
            item.itemName,
            item.icon,
            priceText,
            isBuy,
            !isBuy,
            () => { if (TryBuyItem(item)) Debug.Log($"✅ 成功購買 {item.itemName}"); },
            () => { if (TrySellItem(item)) Debug.Log($"✅ 成功販售 {item.itemName}"); }
        );
    }

    bool TryBuyItem(ShopItemInfo item)
    {
        if (PlayerWallet.Instance == null) return false;

        if (!playerInventory.Add(item.itemData, 1)) return false;

        if (!PlayerWallet.Instance.Spend(item.buyPrice))
        {
            playerInventory.Remove(item.itemData, 1); // 回收物品
            return false;
        }

        return true;
    }

    bool TrySellItem(ShopItemInfo item)
    {
        if (PlayerWallet.Instance == null) return false;

        if (!playerInventory.Remove(item.itemData, 1)) return false;

        PlayerWallet.Instance.Earn(item.sellPrice);
        return true;
    }

    void UpdateMoneyUI(int currentMoney)
    {
        playerMoneyText.text = $"金錢：{currentMoney}";
    }

    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
            Destroy(child.gameObject);
    }
}
