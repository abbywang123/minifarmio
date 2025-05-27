using UnityEditor;
using UnityEngine;

public class CreateShopItems
{
    [MenuItem("Tools/Create Shop Items")]
    public static void CreateAllShopItems()
    {
        string cropFolder = "Assets/Crops";
        string shopItemFolder = "Assets/Resources/ShopItems";
        string itemDataFolder = "Assets/Resources/Items";

        // 確保資料夾存在
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder(shopItemFolder))
            AssetDatabase.CreateFolder("Assets/Resources", "ShopItems");
        if (!AssetDatabase.IsValidFolder(itemDataFolder))
            AssetDatabase.CreateFolder("Assets/Resources", "Items");

        // 載入通用圖示
        Sprite seedIcon = Resources.Load<Sprite>("Tools/Seed");
        Sprite fertilizerIcon = Resources.Load<Sprite>("Tools/Muck");

        if (seedIcon == null)
            Debug.LogWarning("⚠️ 請將 Seed.png 放在 Resources/Tools/ 下");
        if (fertilizerIcon == null)
            Debug.LogWarning("⚠️ 請將 Muck.png 放在 Resources/Tools/ 下");

        // 處理所有作物
        string[] guids = AssetDatabase.FindAssets("t:CropInfo", new[] { cropFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CropInfo crop = AssetDatabase.LoadAssetAtPath<CropInfo>(path);
            if (crop == null) continue;

            // 建立種子商品
            if (!crop.isHybrid)
            {
                ShopItemInfo seed = ScriptableObject.CreateInstance<ShopItemInfo>();
                seed.itemName = crop.cropName + "種子";
                seed.description = $"可用來種植 {crop.cropName}。";
                seed.itemType = ShopItemType.Seed;
                seed.icon = seedIcon;
                seed.linkedCrop = crop;
                seed.buyPrice = EstimateSeedPrice(crop);
                seed.sellPrice = Mathf.RoundToInt(seed.buyPrice * 0.5f);
                seed.canBuy = true;
                seed.canSell = true;

                string seedAssetPath = $"{shopItemFolder}/{seed.itemName}.asset";
                AssetDatabase.CreateAsset(seed, seedAssetPath);
                CreateItemData(seed, itemDataFolder);
            }

            // 建立作物商品
            ShopItemInfo product = ScriptableObject.CreateInstance<ShopItemInfo>();
            product.itemName = crop.cropName;
            product.description = $"成熟的 {crop.cropName}，可以販售。";
            product.itemType = ShopItemType.Crop;
            product.icon = crop.icon;
            product.linkedCrop = crop;
            product.buyPrice = 0;
            product.sellPrice = EstimateCropSellPrice(crop);
            product.canBuy = false;
            product.canSell = true;

            string cropAssetPath = $"{shopItemFolder}/{product.itemName}.asset";
            AssetDatabase.CreateAsset(product, cropAssetPath);
            CreateItemData(product, itemDataFolder);
        }

        // 建立肥料商品
        ShopItemInfo fertilizer = ScriptableObject.CreateInstance<ShopItemInfo>();
        fertilizer.itemName = "通用肥料";
        fertilizer.description = "可以促進作物成長的肥料。";
        fertilizer.itemType = ShopItemType.Fertilizer;
        fertilizer.icon = fertilizerIcon;
        fertilizer.buyPrice = 40;
        fertilizer.sellPrice = 10;
        fertilizer.canBuy = true;
        fertilizer.canSell = true;

        AssetDatabase.CreateAsset(fertilizer, $"{shopItemFolder}/通用肥料.asset");
        CreateItemData(fertilizer, itemDataFolder);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ 商店與背包物品建立完成！");
    }

    private static void CreateItemData(ShopItemInfo shopItem, string folderPath)
    {
        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.id = shopItem.itemName;
        item.itemName = shopItem.itemName;
        item.icon = shopItem.icon;
        item.description = shopItem.description;
        item.maxStack = 99;
        item.itemType = ConvertToItemType(shopItem.itemType);

        string assetPath = $"{folderPath}/{item.itemName}.asset";
        AssetDatabase.CreateAsset(item, assetPath);
    }

    private static int EstimateCropSellPrice(CropInfo crop)
    {
        float basePrice = 50f;
        float growthFactor = 2f / crop.growthRate;
        float hybridBonus = crop.isHybrid ? 30f : 0f;
        float magicBonus = crop.cropType == CropType.Magic ? 20f : 0f;
        return Mathf.RoundToInt(basePrice + growthFactor * 10f + hybridBonus + magicBonus);
    }

    private static int EstimateSeedPrice(CropInfo crop)
    {
        return Mathf.RoundToInt(EstimateCropSellPrice(crop) * 0.6f);
    }

    private static ItemType ConvertToItemType(ShopItemType shopType)
    {
        switch (shopType)
        {
            case ShopItemType.Seed: return ItemType.Seed;
            case ShopItemType.Fertilizer: return ItemType.Fertilizer;
            case ShopItemType.Crop: return ItemType.Crop;
            default: return ItemType.Misc;
        }
    }
}
