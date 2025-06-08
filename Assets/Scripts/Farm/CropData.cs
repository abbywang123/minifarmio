using System.Collections.Generic;
using UnityEngine;

public static class CropDatabase
{
    private static Dictionary<string, CropInfo> crops;

    public static CropInfo GetCropBySeedId(string seedId)
    {
        if (crops == null)
        {
            crops = new Dictionary<string, CropInfo>();
            var all = Resources.LoadAll<CropInfo>("Items");

            foreach (var crop in all)
            {
                if (!string.IsNullOrEmpty(crop.seedId))
                    crops[crop.seedId] = crop;
            }
        }

        if (crops.TryGetValue(seedId, out var result))
            return result;

        Debug.LogError($"[CropDatabase] ❌ 找不到 seedId = {seedId} 的作物資料");
        return null;
    }
}
