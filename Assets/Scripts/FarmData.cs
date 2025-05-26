using System.Collections.Generic;

[System.Serializable]
public class FarmData
{
    public string playerName;
    public int gold;

    public int maxInventorySize = 20; // ✅ 新增背包最大格子數，預設 20

    public List<ItemSlot> inventory;
    public List<FarmlandTile> farmland;
}

[System.Serializable]
public class ItemSlot
{
    public string itemId;
    public int count;
}

[System.Serializable]
public class FarmlandTile
{
    public int x, y;
    public string cropId;
    public int growDays;
    public bool isTilled;
}
