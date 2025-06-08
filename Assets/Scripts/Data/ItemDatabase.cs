using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    private Dictionary<string, ItemData> itemMap;

    void Awake()
    {
        // ✅ 單例初始化
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllItems();
    }

    // ✅ 從 Resources/Items 自動載入所有 ItemData
    void LoadAllItems()
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");

        if (allItems == null || allItems.Length == 0)
        {
            Debug.LogError("❌ Resources/Items 資料夾內沒有找到任何 ItemData！");
            itemMap = new Dictionary<string, ItemData>();
            return;
        }

        itemMap = new Dictionary<string, ItemData>();

        foreach (var item in allItems)
        {
            if (string.IsNullOrWhiteSpace(item.id))
            {
                Debug.LogWarning($"⚠️ ItemData『{item.name}』沒有設定 id，已略過！");
                continue;
            }

            if (itemMap.ContainsKey(item.id))
            {
                Debug.LogWarning($"⚠️ 重複的 id『{item.id}』已存在，來自『{item.name}』，已覆蓋先前資料！");
            }

            itemMap[item.id] = item;
        }

        Debug.Log($"✅ ItemDatabase 載入完成，共載入 {itemMap.Count} 筆 ItemData");
    }

    // ✅ 根據 id 取得完整 ItemData
    public ItemData GetItemData(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning("⚠️ 查詢 itemId 為空！");
            return null;
        }

        if (itemMap.TryGetValue(itemId, out var data))
            return data;

        Debug.LogWarning($"⚠️ 找不到 id = {itemId} 的 ItemData！");
        return null;
    }

    // ✅ 取得顯示名稱
    public string GetItemName(string itemId)
    {
        return GetItemData(itemId)?.itemName ?? "未知物品";
    }

    // ✅ 取得圖示
    public Sprite GetIcon(string itemId)
    {
        return GetItemData(itemId)?.icon;
    }
}
