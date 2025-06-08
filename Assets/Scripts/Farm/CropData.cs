using System.Collections.Generic;
using UnityEngine;

public static class CropDatabase
{
    private static Dictionary<string, CropInfo> crops;

    public static CropInfo GetCropBySeedId(string seedId)
    {
        if (crops == null)
            LoadDatabase();

        if (crops.TryGetValue(seedId, out var result))
            return result;

        Debug.LogError($"[CropDatabase] ❌ 找不到 seedId = {seedId} 的作物資料");
        return null;
    }

    private static void LoadDatabase()
    {
        crops = new Dictionary<string, CropInfo>();

        // ✅ 請確保所有 CropInfo 放在 Resources/Crops/ 內
        var all = Resources.LoadAll<CropInfo>("Crops");

        if (all == null || all.Length == 0)
        {
            Debug.LogWarning("⚠️ CropDatabase 無法從 Resources/Crops/ 載入任何 CropInfo");
            return;
        }

        foreach (var crop in all)
        {
            if (string.IsNullOrEmpty(crop.seedId))
            {
                Debug.LogWarning($"⚠️ CropInfo '{crop.name}' 沒有設定 seedId，已略過");
                continue;
            }

            if (crops.ContainsKey(crop.seedId))
            {
                Debug.LogWarning($"⚠️ 重複的 seedId：{crop.seedId}，已被後者 '{crop.name}' 覆蓋");
            }

            crops[crop.seedId] = crop;
        }

        Debug.Log($"✅ CropDatabase 載入完成，共 {crops.Count} 筆作物資料");
    }
}
