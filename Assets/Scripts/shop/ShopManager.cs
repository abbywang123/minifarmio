using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [Header("UI 元件")]
    public GameObject shopPanel;
    public Button openShopButton;
    public Button buyTabButton;
    public Button sellTabButton;

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
        shopPanel.SetActive(true);

        openShopButton.onClick.RemoveAllListeners();
        openShopButton.onClick.AddListener(ReturnToFarmScene);
        buyTabButton.onClick.AddListener(() => SwitchTab(true));
        sellTabButton.onClick.AddListener(() => SwitchTab(false));

        LoadShopItems();
        SwitchTab(true);
        UpdateMoneyUI();
    }

    void ReturnToFarmScene()
    {
        SceneManager.LoadScene("Farm");
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
                Debug.Log($"嘗試購買 {item.itemName}");
                if (TryBuyItem(item))
                {
                    UpdateMoneyUI();
                    Debug.Log("購買成功");
                }
                else
                    Debug.Log("購買失敗");
            },
            () => {
                Debug.Log($"嘗試賣出 {item.itemName}");
                if (TrySellItem(item))
                {
                    UpdateMoneyUI();
                    Debug.Log("賣出成功");
                }
                else
                    Debug.Log("賣出失敗");
            }
        );
    }

    bool TryBuyItem(ShopItemInfo item)
    {
        if (item == null || item.itemData == null)
        {
            Debug.LogError("物品資料不完整，無法購買");
            return false;
        }

        if (!playerWallet.CanAfford(item.buyPrice))
        {
            Debug.Log("金錢不足");
            return false;
        }

        ItemData realData = ItemDatabase.I.Get(item.itemData.id);
        if (realData == null)
        {
            Debug.LogError($"ItemDatabase找不到ID={item.itemData.id}");
            return false;
        }

        if (!playerInventory.Add(realData, 1))
        {
            Debug.Log("背包無法加入物品");
            return false;
        }

        playerWallet.Spend(item.buyPrice);
        return true;
    }

    bool TrySellItem(ShopItemInfo item)
    {
        if (item == null || item.itemData == null)
        {
            Debug.LogError("物品資料不完整，無法賣出");
            return false;
        }

        ItemData realData = ItemDatabase.I.Get(item.itemData.id);
        if (realData == null)
        {
            Debug.LogError($"ItemDatabase找不到ID={item.itemData.id}");
            return false;
        }

        if (!playerInventory.Remove(realData, 1))
        {
            Debug.Log("背包沒有足夠物品可賣出");
            return false;
        }

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
