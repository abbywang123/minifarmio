using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

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
        SceneManager.sceneLoaded += OnSceneLoaded_ReturnedFromShop;
        SceneManager.LoadScene("Farm");
    }

    void OnSceneLoaded_ReturnedFromShop(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Farm")
        {
            Debug.Log("🟢 從商店回到 Farm，不主動刷新背包 UI（由 InventorySceneManager 控制）");
            SceneManager.sceneLoaded -= OnSceneLoaded_ReturnedFromShop;
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
            async () =>
            {
                Debug.Log($"🛒 嘗試購買 {item.itemName}");
                bool success = await TryBuyItemAsync(item);
                if (success)
                {
                    UpdateMoneyUI(PlayerWallet.Instance.CurrentMoney);
                    Debug.Log("✅ 購買成功");

                    await Task.Delay(1000); // ⏳ 延遲 1 秒避免儲存未完成就切場景
                    ReturnToFarmScene();
                }
                else
                {
                    Debug.Log("❌ 購買失敗");
                }
            },
            async () =>
            {
                Debug.Log($"📤 嘗試賣出 {item.itemName}");
                bool success = await TrySellItemAsync(item);
                if (success)
                {
                    UpdateMoneyUI(PlayerWallet.Instance.CurrentMoney);
                    Debug.Log("✅ 賣出成功");
                }
                else
                {
                    Debug.Log("❌ 賣出失敗");
                }
            }
        );
    }

    async Task<bool> TryBuyItemAsync(ShopItemInfo item)
    {
        if (item == null || item.itemData == null)
        {
            Debug.LogError("❌ 物品資料不完整，無法購買");
            return false;
        }

        if (!playerWallet.CanAfford(item.buyPrice))
        {
            Debug.Log("❌ 金錢不足");
            return false;
        }

        InventoryManager.Instance.AddItemToInventory(item.itemData.id, 1);
        playerWallet.Spend(item.buyPrice);

        try
        {
            Debug.Log("📡 儲存購買結果到雲端...");
            var currentData = InventoryManager.Instance.GetCurrentFarmData();
            currentData.gold = playerWallet.CurrentMoney;
            await CloudSaveAPI.SaveFarmData(currentData);
            Debug.Log("✅ 雲端儲存成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 雲端儲存失敗: {e.Message}");
            return false;
        }

        return true;
    }

    async Task<bool> TrySellItemAsync(ShopItemInfo item)
    {
        if (item == null || item.itemData == null)
        {
            Debug.LogError("❌ 物品資料不完整，無法賣出");
            return false;
        }

        bool removed = await InventoryManager.Instance.RemoveItemAsync(item.itemData.id, 1);
        if (!removed)
        {
            Debug.Log("❌ 背包沒有足夠物品可賣出");
            return false;
        }

        playerWallet.Earn(item.sellPrice);

        try
        {
            Debug.Log("📡 儲存賣出結果到雲端...");
            var currentData = InventoryManager.Instance.GetCurrentFarmData();
            currentData.gold = playerWallet.CurrentMoney;
            await CloudSaveAPI.SaveFarmData(currentData);
            Debug.Log("✅ 雲端儲存成功");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ 雲端儲存失敗: {e.Message}");
            return false;
        }

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

