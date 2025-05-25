using System.Collections.Generic;

[System.Serializable]
public class FarmData
{
    public string playerName;
    public int gold;
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
