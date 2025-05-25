using UnityEditor;
using UnityEngine;

public class CreateShopItems
{
    [MenuItem("Tools/Create Shop Items")]
    public static void CreateAllShopItems()
    {
        string cropFolder = "Assets/Crops";
        string shopItemFolder = "Assets/Resources/ShopItems";

        // 確保資料夾存在
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder(shopItemFolder))
            AssetDatabase.CreateFolder("Assets/Resources", "ShopItems");

        // 載入通用圖片（注意大小寫）
        Sprite seedIcon = Resources.Load<Sprite>("Tools/Seed");
        Sprite fertilizerIcon = Resources.Load<Sprite>("Tools/Muck");

        if (seedIcon == null)
            Debug.LogWarning("⚠️ 無法載入 Seed.png，請確認放置於 Resources/Tools/");
        if (fertilizerIcon == null)
            Debug.LogWarning("⚠️ 無法載入 Muck.png，請確認放置於 Resources/Tools/");

        // 找到所有 CropInfo
        string[] guids = AssetDatabase.FindAssets("t:CropInfo", new[] { cropFolder });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            CropInfo crop = AssetDatabase.LoadAssetAtPath<CropInfo>(path);

            if (crop == null)
                continue;

            // 建立種子（非混種）
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

                AssetDatabase.CreateAsset(seed, $"{shopItemFolder}/{seed.itemName}.asset");
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

            AssetDatabase.CreateAsset(product, $"{shopItemFolder}/{product.itemName}.asset");
        }

        // 建立肥料
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

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ 商店物品建立完成！");
    }

    private static int EstimateCropSellPrice(CropInfo crop)
    {
        float basePrice = 50f;
        float growthFactor = 2f / crop.growthRate;
        float hybridBonus = crop.isHybrid ? 30f : 0f;
        float magicBonus = crop.cropType == CropType.Magic ? 20f : 0f;

        float finalValue = basePrice + growthFactor * 10f + hybridBonus + magicBonus;
        return Mathf.RoundToInt(finalValue);
    }

    private static int EstimateSeedPrice(CropInfo crop)
    {
        return Mathf.RoundToInt(EstimateCropSellPrice(crop) * 0.6f);
    }
}
