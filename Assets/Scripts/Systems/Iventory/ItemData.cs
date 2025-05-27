using UnityEngine;

public enum ItemType
{
    Seed,
    Fertilizer,
    Crop,
    Misc  // 新增這一行
}


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string id;              // 唯一 ID：AuraFertilizer
    public string itemName;        // 顯示名稱：光之肥料
    public Sprite icon;            // 圖示
    public ItemType itemType;      // 類型：Fertilizer
    public int maxStack = 99;      // 疊加數量
    public string description;     // 說明文字

    // 新增屬性，判斷是否可堆疊
    public bool stackable => maxStack > 1;
}
