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

    [Header("設定")]
    public int defaultSlotCount = 20; // ✅ 預設顯示格子數

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
            await SaveInventoryAsync();
            gameObject.SetActive(false); // ✅ 關閉背包 UI
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

        for (int i = 0; i < defaultSlotCount; i++)
        {
            GameObject go = Instantiate(slotPrefab, gridParent);
            go.name = $"Slot_{i}";

            Image iconImage = go.transform.Find("Icon")?.GetComponent<Image>();
            TMP_Text countText = go.transform.Find("CountText")?.GetComponent<TMP_Text>();

            if (i < inventory.syncedInventory.Count)
            {
                var slot = inventory.syncedInventory[i];

                if (iconImage != null)
                    iconImage.sprite = iconMap.ContainsKey(slot.itemId.ToString()) ? iconMap[slot.itemId.ToString()] : defaultIcon;

                if (countText != null)
                    countText.text = $"x{slot.count}";
            }
            else
            {
                // 空格子顯示淡淡的預設圖
                if (iconImage != null)
                {
                    iconImage.sprite = defaultIcon;
                    iconImage.color = new Color(1f, 1f, 1f, 0.3f); // 半透明
                }

                if (countText != null)
                    countText.text = "";
            }
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

