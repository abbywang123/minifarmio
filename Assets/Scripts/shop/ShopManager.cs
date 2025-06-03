using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject shopPanel;                    // 整體商店面板
    public Button openShopButton;                   // 改為返回農場的按鈕
    public Button buyTabButton;                     // 買入分頁按鈕
    public Button sellTabButton;                    // 賣出分頁按鈕

    [Header("ScrollView 面板")]
    public GameObject buyScrollView;
    public GameObject sellScrollView;
    public Transform buyContentParent;
    public Transform sellContentParent;

    [Header("Prefab")]
    public GameObject shopItemUIPrefab;

    [Header("玩家系統")]
    public PlayerWallet playerWallet;
    public Inventory playerInventory;

    [Header("顯示金錢")]
    public TextMeshProUGUI playerMoneyText;

    private ShopItemInfo[] shopItems;

    void Start()
{
    // 1. 啟用商店面板
    shopPanel.SetActive(true);

    // 2. 綁定按鈕事件
    openShopButton.onClick.RemoveAllListeners();
    openShopButton.onClick.AddListener(ReturnToFarmScene);
    buyTabButton.onClick.AddListener(() => SwitchTab(true));
    sellTabButton.onClick.AddListener(() => SwitchTab(false));

    // 3. 加載商品
    LoadShopItems();

    // 4. 顯示買入頁籤
    SwitchTab(true);

    // 5. 更新金錢顯示
    UpdateMoneyUI();
}


    void ReturnToFarmScene()
    {
        Debug.Log("🔙 返回農場場景");
        SceneManager.LoadScene("Farm"); // 確保 Farm 已加入 Build Settings
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
