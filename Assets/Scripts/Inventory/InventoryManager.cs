using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    public string currentlyDraggingItemId = null;

    [Header("UI References")]
    public GameObject slotDraggablePrefab;
    public GameObject addSlotButtonPrefab;
    public Transform gridParent;

    [Header("拖拽所需 Canvas（務必從 Inspector 指定）")]
    public Canvas mainCanvas;

    [Header("Item Info Popup")]
    public GameObject itemInfoPopup;
    public TMP_Text itemNameText;
    public TMP_Text itemDescText;
    public Button useButton;
    public Button discardButton;

    [Header("UI 提示彈窗")]
    public GameObject popupMessage;
    public TMP_Text messageText;

    private List<ItemSlot> inventoryData;
    private FarmData farmData;
    private string currentItemId;

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

    async void Start()
    {
        InitUIReferences();
        Debug.Log("🟡 InventoryManager 啟動");

        await AuthHelper.EnsureSignedIn();
        Debug.Log("✅ 登入完成，開始載入 Cloud Save");
        farmData = await CloudSaveAPI.LoadFarmData();

        if (farmData == null || farmData.inventory == null)
        {
            Debug.LogWarning("📬 Cloud Save 無資料，自動建立新存檔");

            farmData = new FarmData
            {
                playerName = "新玩家",
                gold = 1000,
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
            Debug.Log("✅ 初始存檔已建立並上傳");
        }
        else
        {
            farmData.CleanInvalidSlots();
        }


        inventoryData = farmData.inventory;
        Debug.Log($"📦 載入道具數：{inventoryData?.Count ?? 0}");

        RefreshInventoryUI();
        foreach (var slot in inventoryData)
{
    Debug.Log($"🧾 背包有 {slot.itemId} x{slot.count}");
}

    }

    void OnEnable()
    {
        InitUIReferences();
        if (inventoryData != null && farmData != null)
            RefreshInventoryUI();
    }

    private void InitUIReferences()
    {
        if (gridParent == null)
        {
            GameObject gridObj = GameObject.Find("GridParent");
            if (gridObj != null)
            {
                gridParent = gridObj.transform;
                Debug.Log("🟢 自動繫定 GridParent 成功");
            }
            else
            {
                Debug.LogError("❌ 無法找到 gridParent！");
            }
        }

        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
            Debug.Log("🟢 自動繫定 MainCanvas 成功");
        }

        if (itemInfoPopup == null)
        {
            var popupObj = GameObject.Find("ItemInfoPopup");
            if (popupObj != null)
            {
                itemInfoPopup = popupObj;
                Debug.Log("🟢 自動繫定 itemInfoPopup 成功");
            }
        }

        itemNameText ??= GameObject.Find("ItemNameText")?.GetComponent<TMP_Text>();
        itemDescText ??= GameObject.Find("ItemDescText")?.GetComponent<TMP_Text>();
        useButton ??= GameObject.Find("UseButton")?.GetComponent<Button>();
        discardButton ??= GameObject.Find("DiscardButton")?.GetComponent<Button>();
        popupMessage ??= GameObject.Find("PopupMessage");
        messageText ??= GameObject.Find("MessageText")?.GetComponent<TMP_Text>();

        if (addSlotButtonPrefab == null)
        {
            var prefab = Resources.Load<GameObject>("AddSlotButton");
            if (prefab != null)
            {
                addSlotButtonPrefab = prefab;
                Debug.Log("🟢 自動繫定 AddSlotButtonPrefab 成功");
            }
        }
    }

    public void RefreshInventoryUI()
    {
        InitUIReferences();

        if (gridParent == null)
        {
            Debug.LogError("❌ 無法找到 gridParent！");
            return;
        }

        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        for (int i = 0; i < farmData.maxInventorySize; i++)
        {
            GameObject go = Instantiate(slotDraggablePrefab, gridParent);
            var ui = go.GetComponent<InventorySlotUI>();
            ui.canvas = mainCanvas;

            if (i < inventoryData.Count)
            {
                var slot = inventoryData[i];
                ui.Setup(slot);
                ui.EnableDragging();
            }
            else
            {
                ui.Setup("", 0);
            }
        }

        if (addSlotButtonPrefab != null)
        {
            GameObject addBtn = Instantiate(addSlotButtonPrefab, gridParent);
            addBtn.name = "AddSlotButton";
            var btn = addBtn.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => _ = OnClickAddSlot());
        }
    }

    public void ShowItemInfo(string itemId, int count)
    {
        InitUIReferences();

        if (itemInfoPopup == null)
        {
            Debug.LogError("❌ 無法顯示 itemInfoPopup");
            return;
        }

        if (itemInfoPopup.activeSelf && itemId == currentItemId)
        {
            itemInfoPopup.SetActive(false);
            currentItemId = null;
            return;
        }

        currentItemId = itemId;
        itemInfoPopup.SetActive(true);

        itemNameText.text = $"itemId: {itemId}";
        itemDescText.text = $"count: {count}";
        useButton?.gameObject.SetActive(false);

        discardButton?.onClick.RemoveAllListeners();
        discardButton?.onClick.AddListener(() => DiscardItem(itemId));
    }

    void DiscardItem(string itemId)
    {
        var item = inventoryData.Find(slot => slot.itemId == itemId);
        if (item != null)
        {
            item.count--;
            if (item.count <= 0) inventoryData.Remove(item);
            _ = SaveInventoryThenRefresh();
        }
    }
    public void AddItemToInventory(string itemId, int count)
{
    if (farmData == null || inventoryData == null)
    {
        Debug.LogWarning("❌ farmData 或 inventoryData 為 null，無法加入道具");
        return;
    }

    var slot = inventoryData.Find(s => s.itemId == itemId);

    if (slot != null)
    {
        slot.count += count;
    }
    else
    {
        inventoryData.Add(new ItemSlot { itemId = itemId, count = count });
    }

    Debug.Log($"✅ 新增道具 {itemId} × {count} 到背包");

    _ = SaveInventoryThenRefresh();  // 非同步儲存並刷新 UI
}


    async Task SaveInventoryThenRefresh()
    {
        itemInfoPopup?.SetActive(false);
        FarmData latest = await CloudSaveAPI.LoadFarmData();
        latest.inventory = inventoryData;
        latest.maxInventorySize = farmData.maxInventorySize;
        latest.gold = farmData.gold;

        await CloudSaveAPI.SaveFarmData(latest);
        RefreshInventoryUI();
    }

    async Task OnClickAddSlot()
    {
        const int cost = 100;
        const int maxSlots = 40;

        var latest = await CloudSaveAPI.LoadFarmData();
        latest?.CleanInvalidSlots();

        if (latest == null)
        {
            ShowPopup("❌ 無法從雲端取得存檔");
            return;
        }

        farmData = latest;
        inventoryData = farmData.inventory;

        if (farmData.maxInventorySize >= maxSlots)
        {
            ShowPopup($"❌ 已達上限 {maxSlots} 格");
            return;
        }

        if (farmData.gold < cost)
        {
            ShowPopup($"💰 金幣不足（需 {cost}）");
            return;
        }

        farmData.gold -= cost;
        farmData.maxInventorySize++;

        ShowPopup($"✅ 擴充成功！剩餘金幣：{farmData.gold}");

        await CloudSaveAPI.SaveFarmData(farmData);
        RefreshInventoryUI();
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
        popupMessage?.SetActive(false);
    }

    public List<ItemSlot> GetInventoryData() => inventoryData;
    public string GetDraggingItem() => currentlyDraggingItemId;
    public void SetDraggingItem(string itemId) => currentlyDraggingItemId = itemId;
    public void ClearDraggingItem() => currentlyDraggingItemId = null;

    public async Task ReloadFarmDataFromCloud()
    {
        Debug.Log("🔄 正在重新從雲端載入 farmData");
        farmData = await CloudSaveAPI.LoadFarmData();
        farmData?.CleanInvalidSlots();
        inventoryData = farmData?.inventory;
        RefreshInventoryUI();
    }

    public bool RemoveItem(string itemId, int count = 1)
    {
        var item = inventoryData.Find(slot => slot.itemId == itemId);
        if (item == null || item.count < count)
            return false;

        item.count -= count;
        if (item.count <= 0)
            inventoryData.Remove(item);

        _ = SaveInventoryThenRefresh();
        return true;
    }

}






