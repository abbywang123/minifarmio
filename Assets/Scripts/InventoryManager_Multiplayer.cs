using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.Netcode;
using System.Threading.Tasks;

public class InventoryManager_Multiplayer : MonoBehaviour
{
    [Header("UI References")]
    public GameObject slotPrefab;
    public Transform gridParent;
    public Button backToFarmButton;

    [Header("Icon Resources")]
    public Sprite defaultIcon;
    public Sprite wheatIcon;
    public Sprite carrotIcon;

    private Dictionary<string, Sprite> iconMap;

    void Start()
    {
        Debug.Log("🟢 Multiplayer Inventory UI 啟動");

        iconMap = new Dictionary<string, Sprite>
        {
            { "wheat", wheatIcon },
            { "carrot", carrotIcon }
        };

        backToFarmButton.onClick.AddListener(async () =>
        {
            await SaveInventoryAsync(); // ✅ 加入儲存
            gameObject.SetActive(false); // ✅ 關閉 UI
        });

        RefreshInventoryUI();
    }

    void RefreshInventoryUI()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        var player = NetworkManager.Singleton.LocalClient?.PlayerObject;
        if (player == null)
        {
            Debug.LogWarning("⚠️ 找不到本地玩家 NetworkObject");
            return;
        }

        var inventory = player.GetComponent<PlayerInventorySync>();
        if (inventory == null)
        {
            Debug.LogWarning("⚠️ 找不到 PlayerInventorySync");
            return;
        }

        foreach (var slot in inventory.syncedInventory)
        {
            GameObject go = Instantiate(slotPrefab, gridParent);

            Image iconImage = go.transform.Find("Icon")?.GetComponent<Image>();
            if (iconImage != null)
                iconImage.sprite = iconMap.ContainsKey(slot.itemId.ToString()) ? iconMap[slot.itemId.ToString()] : defaultIcon;

            TMP_Text countText = go.transform.Find("CountText")?.GetComponent<TMP_Text>();
            if (countText != null)
                countText.text = $"x{slot.count}";

            go.name = $"Slot_{slot.itemId}";
        }
    }

    // ✅ 儲存同步背包資料到 Cloud Save
    async Task SaveInventoryAsync()
    {
        var player = NetworkManager.Singleton.LocalClient?.PlayerObject;
        if (player == null) return;

        var inventory = player.GetComponent<PlayerInventorySync>();
        if (inventory == null) return;

        // 轉換 NetworkList → List<ItemSlot>
        List<ItemSlot> itemList = new();
        foreach (var slot in inventory.syncedInventory)
        {
            itemList.Add(new ItemSlot
            {
                itemId = slot.itemId.ToString(),
                count = slot.count
            });
        }

        FarmData data = await CloudSaveAPI.LoadFarmData();
        data.inventory = itemList;
        await CloudSaveAPI.SaveFarmData(data);

        Debug.Log("✅ 背包已儲存回 Cloud Save");
    }
}

