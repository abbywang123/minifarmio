using System.Collections.Generic;
using UnityEngine;

public static class CropDatabase
{
    private static Dictionary<string, CropInfo> crops;

    public static CropInfo Get(string name)
    {
        if (crops == null)
        {
            crops = new Dictionary<string, CropInfo>();
           var all = Resources.LoadAll<CropInfo>("Items");

            foreach (var crop in all)
            {
                crops[crop.name] = crop;
            }
        }

        if (crops.TryGetValue(name, out var result))
            return result;

        Debug.LogError($"[CropDatabase] 找不到名稱為「{name}」的作物資料");
        return null;
    }
}
