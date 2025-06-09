using System;
using System.Collections.Generic;

[Serializable]
public class FarmData
{
    public string playerName;
    public int gold;

    public int maxInventorySize = 12; // ✅ 預設最大背包格子數

    public List<ItemSlot> inventory = new(); // ✅ 初始化避免 null
    public List<FarmlandTile> farmland = new();

    /// <summary>
    /// ✅ 清除無效欄位，避免 itemId 為空或 FarmlandTile 無效導致錯誤
    /// </summary>
    public void CleanInvalidSlots()
    {
        inventory.RemoveAll(slot => string.IsNullOrEmpty(slot.itemId));
        farmland.RemoveAll(tile =>
            tile == null ||           // tile 本身為 null
            tile.x < 0 || tile.y < 0 || // 座標非法（你可依需求調整）
            tile.cropId == null        // cropId 不該為 null
        );
    }
}

[Serializable]
public class ItemSlot
{
    public string itemId;
    public int count;
}

[Serializable]
public class FarmlandTile
{
    public int x, y;               // 格子座標（World Space or Grid）
    public string cropId;         // 對應 CropInfo 的 seedId
    public int growDays;          // 已經生長的天數
    public bool isTilled;         // 是否已翻土

    // ✅ 可擴充欄位（例如）
    // public DateTime plantedAt;
    // public bool isWatered;
    // public string plantedByPlayerId;
}
