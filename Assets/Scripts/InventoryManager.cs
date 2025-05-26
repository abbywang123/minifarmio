using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject slotPrefab;
    public Transform gridParent;

    [Header("Icon Resources")]
    public Sprite defaultIcon;
    public Sprite wheatIcon;
    public Sprite carrotIcon;

    [Header("Item Info Popup")]
    public GameObject itemInfoPopup;
    public TMP_Text itemNameText;
    public TMP_Text itemDescText;
    public Button useButton;
    public Button discardButton;

    private Dictionary<string, Sprite> iconMap;
    private List<ItemSlot> inventoryData;
    private string currentItemId;

    async void Start()
    {
        Debug.Log("🟡 InventoryManager 啟動");

        // ✅ 等待登入初始化完成（改用 AuthHelper）
        await AuthHelper.EnsureSignedIn();

        Debug.Log("✅ 登入完成，開始載入 Cloud Save");

        // ✅ 載入 Cloud Save 資料（自動初始化）
        FarmData farmData = await CloudSaveAPI.LoadFarmData();

        if (farmData == null)
        {
            Debug.LogWarning("📭 Cloud Save 無資料，自動建立新存檔");

            farmData = new FarmData
            {
                playerName = "新玩家",
                gold = 999,
                inventory = new List<ItemSlot>
                {
                    new ItemSlot { itemId = "wheat", count = 3 },
                    new ItemSlot { itemId = "carrot", count = 5 }
                },
                farmland = new List<FarmlandTile>()
            };

            await CloudSaveAPI.SaveFarmData(farmData);
            Debug.Log("✅ 初始存檔已建立");
        }

        inventoryData = farmData.inventory;
        Debug.Log($"📦 載入道具數：{inventoryData?.Count ?? 0}");

        // ✅ 建立圖示對照表
        iconMap = new Dictionary<string, Sprite>
        {
            { "wheat", wheatIcon },
            { "carrot", carrotIcon }
        };

        RefreshInventoryUI();
    }

    void RefreshInventoryUI()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        foreach (var slot in inventoryData)
        {
            GameObject go = Instantiate(slotPrefab, gridParent);
            go.name = $"Slot_{slot.itemId}";

            Image iconImage = go.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
                iconImage.sprite = iconMap.ContainsKey(slot.itemId) ? iconMap[slot.itemId] : defaultIcon;

            TMP_Text countText = go.transform.Find("CountText")?.GetComponent<TMP_Text>();
            if (countText != null)
                countText.text = $"x{slot.count}";

            string id = slot.itemId;
            int count = slot.count;
            go.GetComponent<Button>().onClick.AddListener(() => ShowItemInfo(id, count));
        }
    }

    void ShowItemInfo(string itemId, int count)
    {
        currentItemId = itemId;
        itemInfoPopup.SetActive(true);

        itemNameText.text = itemId switch
        {
            "wheat" => "小麥",
            "carrot" => "紅蘿蔔",
            _ => "未知物品"
        };

        itemDescText.text = $"你擁有 {count} 個";

        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(() => UseItem(itemId));

        discardButton.onClick.RemoveAllListeners();
        discardButton.onClick.AddListener(() => DiscardItem(itemId));
    }

    void UseItem(string itemId)
    {
        Debug.Log($"🧪 使用物品：{itemId}");
        // 可擴充功能：使用道具
    }

    void DiscardItem(string itemId)
    {
        Debug.Log($"🗑️ 丟棄物品：{itemId}");
        inventoryData.RemoveAll(item => item.itemId == itemId);
        _ = SaveInventoryThenRefresh();
    }

    async Task SaveInventoryThenRefresh()
    {
        FarmData farmData = await CloudSaveAPI.LoadFarmData();
        farmData.inventory = inventoryData;

        await CloudSaveAPI.SaveFarmData(farmData);
        itemInfoPopup.SetActive(false);
        RefreshInventoryUI();
    }

    public List<ItemSlot> GetInventoryData()
    {
        return inventoryData;
    }
}

