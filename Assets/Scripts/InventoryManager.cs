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
    public Sprite turnipIcon;
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
        // ✅ 載入整份資料
        FarmData farmData = await CloudSaveAPI.LoadFarmData();
        inventoryData = farmData.inventory;

        // ✅ 建立圖示對照表
        iconMap = new Dictionary<string, Sprite>
        {
            { "turnip", turnipIcon },
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
            "turnip" => "蘿蔔",
            "carrot" => "紅蘿蔔",
            _ => "未知道具"
        };
        itemDescText.text = $"你擁有 {count} 個\n這是一個神奇的 {itemNameText.text}。";

        useButton.onClick.RemoveAllListeners();
        discardButton.onClick.RemoveAllListeners();

        useButton.onClick.AddListener(() => _ = UseItem(currentItemId));
        discardButton.onClick.AddListener(() => _ = DiscardItem(currentItemId));
    }

    async Task UseItem(string itemId)
    {
        Debug.Log($"🧪 使用道具：{itemId}");

        // ✅ 先讀完整資料
        FarmData farmData = await CloudSaveAPI.LoadFarmData();
        ItemSlot slot = farmData.inventory.Find(s => s.itemId == itemId);
        if (slot != null && slot.count > 0)
        {
            slot.count--;
            if (slot.count == 0)
                farmData.inventory.Remove(slot);

            await CloudSaveAPI.SaveFarmData(farmData);
            inventoryData = farmData.inventory;
            RefreshInventoryUI();
        }

        itemInfoPopup.SetActive(false);
    }

    async Task DiscardItem(string itemId)
    {
        Debug.Log($"🗑️ 丟棄道具：{itemId}");

        // ✅ 先讀完整資料
        FarmData farmData = await CloudSaveAPI.LoadFarmData();
        ItemSlot slot = farmData.inventory.Find(s => s.itemId == itemId);
        if (slot != null)
        {
            farmData.inventory.Remove(slot);

            await CloudSaveAPI.SaveFarmData(farmData);
            inventoryData = farmData.inventory;
            RefreshInventoryUI();
        }

        itemInfoPopup.SetActive(false);
    }

    // ✅ 新增這個公開方法供 SceneNavigator 存取資料
    public List<ItemSlot> GetInventoryData()
    {
        return inventoryData;
    }
}
