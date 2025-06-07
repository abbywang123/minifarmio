using System.Collections.Generic;
using UnityEngine;

public static class CropIconDatabase
{
    private static Dictionary<string, Sprite> cropIconMap;

    public static void Init()
    {
        cropIconMap = new Dictionary<string, Sprite>();

        Sprite[] sprites = Resources.LoadAll<Sprite>("Crops/picture");

        foreach (var sprite in sprites)
        {
            string id = sprite.name.ToLower();
            cropIconMap[id] = sprite;
        }

        Debug.Log($"✅ 載入作物圖示數量：{cropIconMap.Count}");
    }

    public static Sprite GetSpriteById(string id)
    {
        if (cropIconMap == null)
            Init();

        return cropIconMap.TryGetValue(id.ToLower(), out var sprite) ? sprite : null;
    }
}
