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

        PlayerWallet.Instance.OnMoneyChanged += UpdateMoneyUI;
        UpdateMoneyUI(PlayerWallet.Instance.CurrentMoney);
        LoadShopItems();
        SwitchTab(true);
    }

    void ReturnToFarmScene()
    {
        SceneManager.sceneLoaded += OnSceneLoaded; // 🔧 加入回場景時的刷新
        SceneManager.LoadScene("Farm");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Farm")
        {
            InventoryManager.Instance?.RefreshInventoryUI();  // ✅ 回場景時強制刷新背包
            SceneManager.sceneLoaded -= OnSceneLoaded;        // ✅ 解除事件避免重複
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
            () =>
            {
                Debug.Log($"嘗試購買 {item.itemName}");
                if (TryBuyItem(item))
                {
                    UpdateMoneyUI(PlayerWallet.Instance.CurrentMoney);
                    Debug.Log("購買成功");
                }
                else
                    Debug.Log("購買失敗");
            },
            () =>
            {
                Debug.Log($"嘗試賣出 {item.itemName}");
                if (TrySellItem(item))
                {
                    UpdateMoneyUI(PlayerWallet.Instance.CurrentMoney);
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

        InventoryManager.Instance.AddItemToInventory(item.itemData.id, 1);
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

        if (!InventoryManager.Instance.RemoveItem(item.itemData.id, 1))
        {
            Debug.Log("背包沒有足夠物品可賣出");
            return false;
        }

        playerWallet.Earn(item.sellPrice);
        return true;
    }

    void UpdateMoneyUI(int newMoney)
    {
        playerMoneyText.text = $"金錢：{newMoney}";
    }

    void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
