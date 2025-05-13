using System.Collections.Generic;
using UnityEngine;

public static class CropDatabase
{
    private static Dictionary<string, CropData> crops;

    public static CropData Get(string name)
    {
        if (crops == null)
        {
            crops = new Dictionary<string, CropData>();
            var all = Resources.LoadAll<CropData>("Crops"); // 放在 Resources/Crops 資料夾
            foreach (var crop in all)
            {
                crops[crop.name] = crop;
            }
        }
        return crops[name];
    }
}
