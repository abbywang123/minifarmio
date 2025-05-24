using UnityEngine;

public enum ShopItemType
{
    Seed,
    Fertilizer,
    Crop
}

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Create Shop Item")]
public class ShopItemInfo : ScriptableObject
{
    public string itemName;

    [TextArea(2, 5)]
    public string description;

    public ShopItemType itemType;

    public int buyPrice;
    public int sellPrice;

    public CropInfo linkedCrop; // 若是種子或作物，需關聯
}
