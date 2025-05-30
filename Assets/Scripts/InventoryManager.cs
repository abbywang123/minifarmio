using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;

public class InventoryManager : MonoBehaviour
{
    // ✅ Singleton
    public static InventoryManager Instance { get; private set; }

    // ✅ 跨場景記住目前正在拖曳的物品 ID（如 "carrotseed"）
    public string currentlyDraggingItemId = null;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Header("UI References")]
    public GameObject slotPrefab;
    public GameObject addSlotButtonPrefab;
    public Transform gridParent;

    [Header("Icon Resources")]
    public Sprite defaultIcon;
    public Sprite wheatIcon;
    public Sprite carrotIcon;
    public Sprite carrotSeedIcon;

    [Header("Item Info Popup")]
    public GameObject itemInfoPopup;
    public TMP_Text itemNameText;
    public TMP_Text itemDescText;
    public Button useButton;
    public Button discardButton;

    [Header("UI 提示彈窗")]
    public GameObject popupMessage;
    public TMP_Text messageText;

    public Dictionary<string, Sprite> IconMap => iconMap;

    private Dictionary<string, Sprite> iconMap;
    private List<ItemSlot> inventoryData;
    private FarmData farmData;
    private string currentItemId;

    async void Start()
    {
        Debug.Log("🟡 InventoryManager 啟動");

        await AuthHelper.EnsureSignedIn();
        Debug.Log("✅ 登入完成，開始載入 Cloud Save");

        farmData = await CloudSaveAPI.LoadFarmData();

        if (farmData == null)
        {
            Debug.LogWarning("📬 Cloud Save 無資料，自動建立新存檔");

            farmData = new FarmData
            {
                playerName = "新玩家",
                gold = 999,
                maxInventorySize = 12,
                inventory = new List<ItemSlot>
                {
                    new ItemSlot { itemId = "wheat", count = 3 },
                    new ItemSlot { itemId = "carrot", count = 5 },
                    new ItemSlot { itemId = "carrotseed", count = 10 }
                },
                farmland = new List<FarmlandTile>()
            };

            await CloudSaveAPI.SaveFarmData(farmData);
            Debug.Log("✅ 初始存檔已建立");
        }

        inventoryData = farmData.inventory;
        Debug.Log($"📦 載入道具數：{inventoryData?.Count ?? 0}");

        iconMap = new Dictionary<string, Sprite>
        {
            { "wheat", wheatIcon },
            { "carrot", carrotIcon },
            { "carrotseed", carrotSeedIcon }
        };

        RefreshInventoryUI();
    }

    void RefreshInventoryUI()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        for (int i = 0; i < farmData.maxInventorySize; i++)
        {
            GameObject go = Instantiate(slotPrefab, gridParent);

            if (i < inventoryData.Count)
            {
                var slot = inventoryData[i];
                go.name = $"Slot_{slot.itemId}";

                Image iconImage = go.transform.Find("Icon")?.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = iconMap.ContainsKey(slot.itemId) ? iconMap[slot.itemId] : defaultIcon;

                    if (!iconImage.GetComponent<CanvasGroup>())
                        iconImage.gameObject.AddComponent<CanvasGroup>();

                    DraggableItemSlot drag = iconImage.GetComponent<DraggableItemSlot>();
                    if (drag == null)
                        drag = iconImage.gameObject.AddComponent<DraggableItemSlot>();

                    drag.canvas = GetComponentInParent<Canvas>();
                    drag.itemId = slot.itemId;
                }

                TMP_Text countText = go.transform.Find("CountText")?.GetComponent<TMP_Text>();
                if (countText != null)
                    countText.text = $"x{slot.count}";

                string id = slot.itemId;
                int count = slot.count;
                go.GetComponent<Button>().onClick.AddListener(() => ShowItemInfo(id, count));
            }
            else
            {
                go.name = "Slot_Empty";

                Image iconImage = go.transform.Find("Icon")?.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = defaultIcon;
                    iconImage.color = new Color(1f, 1f, 1f, 0.3f);
                }

                TMP_Text countText = go.transform.Find("CountText")?.GetComponent<TMP_Text>();
                if (countText != null)
                    countText.text = "";
            }
        }

        if (addSlotButtonPrefab != null)
        {
            GameObject addBtn = Instantiate(addSlotButtonPrefab, gridParent);
            addBtn.name = "AddSlotButton";
            addBtn.GetComponent<Button>().onClick.AddListener(OnClickAddSlot);
        }
        else
        {
            Debug.LogWarning("❌ addSlotButtonPrefab 未設定");
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
            "carrotseed" => "紅蘿蔔種子",
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

        var item = inventoryData.Find(slot => slot.itemId == itemId);

        if (item != null)
        {
            item.count--;

            if (item.count <= 0)
                inventoryData.Remove(item);

            _ = SaveInventoryThenRefresh();
        }
    }

    void DiscardItem(string itemId)
    {
        Debug.Log($"🗑️ 丟棄物品：{itemId}");

        var item = inventoryData.Find(slot => slot.itemId == itemId);

        if (item != null)
        {
            item.count--;

            if (item.count <= 0)
                inventoryData.Remove(item);

            _ = SaveInventoryThenRefresh();
        }
    }

    async Task SaveInventoryThenRefresh()
    {
        FarmData latest = await CloudSaveAPI.LoadFarmData();
        latest.inventory = inventoryData;
        latest.maxInventorySize = farmData.maxInventorySize;
        latest.gold = farmData.gold;
        await CloudSaveAPI.SaveFarmData(latest);

        itemInfoPopup.SetActive(false);
        RefreshInventoryUI();
    }

    void OnClickAddSlot()
    {
        const int cost = 100;
        const int maxSlots = 40;

        if (farmData.maxInventorySize >= maxSlots)
        {
            ShowPopup($"❌ 已達最大上限 {maxSlots} 格，無法再擴充！");
            return;
        }

        if (farmData.gold < cost)
        {
            ShowPopup($"💰 金幣不足！需要 {cost} 金幣才能擴充");
            return;
        }

        farmData.gold -= cost;
        farmData.maxInventorySize += 1;

        Debug.Log($"👛 擴充成功，目前 {farmData.maxInventorySize} 格，剩餘金幣：{farmData.gold}");
        ShowPopup($"✅ 擴充成功！剩餘金幣：{farmData.gold}");

        _ = SaveInventoryThenRefresh();
    }

    void ShowPopup(string msg, float duration = 2f)
    {
        if (popupMessage == null || messageText == null) return;

        popupMessage.SetActive(true);
        messageText.text = msg;
        CancelInvoke(nameof(HidePopup));
        Invoke(nameof(HidePopup), duration);
    }

    void HidePopup()
    {
        popupMessage.SetActive(false);
    }

    public List<ItemSlot> GetInventoryData()
    {
        return inventoryData;
    }

    // ✅ 提供取得拖曳中的物品 ID
    public string GetDraggingItem()
    {
        return currentlyDraggingItemId;
    }

    // ✅ 設定拖曳中的物品 ID
    public void SetDraggingItem(string itemId)
    {
        currentlyDraggingItemId = itemId;
    }

    // ✅ 清除拖曳中的物品 ID
    public void ClearDraggingItem()
    {
        currentlyDraggingItemId = null;
    }
}


