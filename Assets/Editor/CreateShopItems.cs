using UnityEditor;
using UnityEngine;

public class CreateShopItems
{
    [MenuItem("Tools/Create Shop Items")]
    public static void CreateAllShopItems()
    {
        string cropFolder = "Assets/Crops";
        string shopItemFolder = "Assets/Resources/ShopItems";

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder(shopItemFolder))
            AssetDatabase.CreateFolder("Assets/Resources", "ShopItems");

        string[] guids = AssetDatabase.FindAssets("t:CropInfo", new[] { cropFolder });
        Debug.Log("找到 Crop 數量：" + guids.Length);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CropInfo crop = AssetDatabase.LoadAssetAtPath<CropInfo>(path);

            if (crop == null) continue;

            Debug.Log($"➡️ 處理作物：{crop.cropName}");

            // 建立可購買的種子（非混種）
            if (!crop.isHybrid)
            {
                ShopItemInfo seed = ScriptableObject.CreateInstance<ShopItemInfo>();
                seed.itemName = crop.cropName + "種子";
                seed.itemType = ShopItemType.Seed;
                seed.linkedCrop = crop;
                seed.icon = crop.icon;

                seed.price = EstimateSeedPrice(crop);
                seed.sellPrice = Mathf.RoundToInt(seed.price * 0.5f);
                seed.canBuy = true;
                seed.canSell = true;

                AssetDatabase.CreateAsset(seed, $"{shopItemFolder}/{seed.itemName}.asset");
            }
            else
            {
                // 混種種子只能販售
                ShopItemInfo hybridSeed = ScriptableObject.CreateInstance<ShopItemInfo>();
                hybridSeed.itemName = crop.cropName + "種子";
                hybridSeed.itemType = ShopItemType.Seed;
                hybridSeed.linkedCrop = crop;
                hybridSeed.icon = crop.icon;

                hybridSeed.price = 0;
                hybridSeed.sellPrice = 50;
                hybridSeed.canBuy = false;
                hybridSeed.canSell = true;

                AssetDatabase.CreateAsset(hybridSeed, $"{shopItemFolder}/{hybridSeed.itemName}.asset");
            }

            // 建立成熟作物販售品
            ShopItemInfo produce = ScriptableObject.CreateInstance<ShopItemInfo>();
            produce.itemName = crop.cropName;
            produce.itemType = ShopItemType.Crop;
            produce.linkedCrop = crop;
            produce.icon = crop.icon;

            produce.price = 0;
            produce.sellPrice = EstimateCropSellPrice(crop);
            produce.canBuy = false;
            produce.canSell = true;

            AssetDatabase.CreateAsset(produce, $"{shopItemFolder}/{produce.itemName}.asset");
        }

        // 建立肥料
        ShopItemInfo fertilizer = ScriptableObject.CreateInstance<ShopItemInfo>();
        fertilizer.itemName = "肥料";
        fertilizer.itemType = ShopItemType.Fertilizer;
        fertilizer.price = 40;
        fertilizer.sellPrice = 10;
        fertilizer.canBuy = true;
        fertilizer.canSell = true;

        AssetDatabase.CreateAsset(fertilizer, $"{shopItemFolder}/肥料.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ 商店物品建立完成！");
    }

    private static int EstimateCropSellPrice(CropInfo crop)
    {
        float basePrice = 50f;
        float growthFactor = 2f / crop.growthRate;
        float tempRange = crop.suitableMaxTemperature - crop.suitableMinTemperature;
        float humiRange = crop.suitableMaxHumidity - crop.suitableMinHumidity;

        float tempDifficulty = 10f / Mathf.Max(0.1f, tempRange);
        float humiDifficulty = 10f / Mathf.Max(0.1f, humiRange);
        float hybridBonus = crop.isHybrid ? 30f : 0f;
        float magicBonus = crop.cropType == CropType.Magic ? 20f : 0f;

        float finalValue = basePrice + growthFactor * 10f + tempDifficulty + humiDifficulty + hybridBonus + magicBonus;
        return Mathf.RoundToInt(finalValue);
    }

    private static int EstimateSeedPrice(CropInfo crop)
    {
        return Mathf.RoundToInt(EstimateCropSellPrice(crop) * 0.6f);
    }
}
