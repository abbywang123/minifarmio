using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateShopItems
{
    [MenuItem("Tools/Create All Shop Items")]
    public static void CreateAllShopItems()
    {
        string cropPath = "Crops";
        string outputPath = "Assets/Resources/ShopItems";

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        if (!AssetDatabase.IsValidFolder(outputPath))
            AssetDatabase.CreateFolder("Assets/Resources", "ShopItems");

        var crops = Resources.LoadAll<CropInfo>(cropPath);

        foreach (var crop in crops)
        {
            // Create Seed Item
            var seed = ScriptableObject.CreateInstance<ShopItemInfo>();
            seed.itemName = crop.cropName + "種子";
            seed.itemType = ShopItemType.Seed;
            seed.linkedCrop = crop;
            seed.description = $"可以種出{crop.cropName}的種子";

            float tempRange = crop.suitableMaxTemperature - crop.suitableMinTemperature;
            float humidityRange = crop.suitableMaxHumidity - crop.suitableMinHumidity;
            float difficulty = (2f - crop.growthRate) + (10f - tempRange) * 0.1f + (1f - humidityRange);
            seed.sellPrice = Mathf.Clamp(Mathf.RoundToInt(difficulty * 20f), 10, 100);
            seed.buyPrice = seed.sellPrice + 5;

            AssetDatabase.CreateAsset(seed, $"{outputPath}/{seed.itemName}.asset");

            // Create Crop Item
            var harvest = ScriptableObject.CreateInstance<ShopItemInfo>();
            harvest.itemName = crop.cropName;
            harvest.itemType = ShopItemType.Crop;
            harvest.linkedCrop = crop;
            harvest.description = $"{crop.cropName} 收成品";
            harvest.sellPrice = seed.sellPrice + 10;
            harvest.buyPrice = 0; // 作物通常不能直接購買

            AssetDatabase.CreateAsset(harvest, $"{outputPath}/{harvest.itemName}.asset");
        }

        // Create Fertilizer
        var fertilizer = ScriptableObject.CreateInstance<ShopItemInfo>();
        fertilizer.itemName = "肥料";
        fertilizer.itemType = ShopItemType.Fertilizer;
        fertilizer.description = "可提升作物品質的肥料";
        fertilizer.buyPrice = 20;
        fertilizer.sellPrice = 5;
        AssetDatabase.CreateAsset(fertilizer, $"{outputPath}/肥料.asset");

        AssetDatabase.SaveAssets();
        Debug.Log("✅ 商店物品已建立完畢！");
    }
}
