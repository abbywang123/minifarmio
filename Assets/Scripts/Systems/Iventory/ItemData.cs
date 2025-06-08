using UnityEngine;

public enum ItemType
{
    Seed,
    Fertilizer,
    Crop,
    Misc  // 雜項（可擴充）
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("基本資料")]
    public string id;               // ✅ 唯一ID，全小寫：如 "carrotseed"
    public string itemName;        // ✅ 中文名稱：如 "紅蘿蔔種子"
    [TextArea(2, 4)]
    public string description;     // ✅ 說明文字

    [Header("外觀")]
    public Sprite icon;            // ✅ 對應 UI 顯示圖

    [Header("屬性")]
    public ItemType itemType;      // ✅ 類別：種子/作物/肥料
    public int maxStack = 99;      // ✅ 可疊加數量

    public bool stackable => maxStack > 1;  // ✅ 自動判斷是否可堆疊
}
