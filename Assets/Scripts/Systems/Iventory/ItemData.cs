using UnityEngine;

public enum ItemType
{
    Seed,
    Fertilizer,
    Crop,
    Misc // 雜項（可擴充）
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("🔑 基本資料")]
    [Tooltip("唯一 ID，全小寫，例如：carrotseed")]
    public string id;

    [Tooltip("物品名稱，例如：紅蘿蔔種子")]
    public string itemName;

    [Tooltip("物品說明文字")]
    [TextArea(2, 4)]
    public string description;

    [Header("🎨 外觀")]
    [Tooltip("顯示於 UI 的圖示")]
    public Sprite icon;

    [Header("⚙️ 屬性")]
    [Tooltip("物品類型")]
    public ItemType itemType;

    [Tooltip("最大堆疊數量")]
    public int maxStack = 99;

    // ✅ 自動判斷是否可堆疊（不需序列化）
    public bool stackable => maxStack > 1;
}

