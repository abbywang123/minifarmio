// Assets/Scripts/IconDatabase.cs

using UnityEngine;
using System.Collections.Generic;

public static class IconDatabase
{
    private static Dictionary<string, Sprite> iconMap = new Dictionary<string, Sprite>();

    // 靜態建構子：第一次使用就會自動執行
    static IconDatabase()
    {
        LoadAllIcons();
    }

    // 載入 Resources/Crops/picture 內所有圖片
    private static void LoadAllIcons()
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Crops/picture");

        if (allSprites.Length == 0)
        {
            Debug.LogWarning("⚠️ IconDatabase: 沒有載入到任何圖示，請確認路徑是否為 Assets/Resources/Crops/picture/");
        }

        foreach (var sprite in allSprites)
        {
            if (!iconMap.ContainsKey(sprite.name))
            {
                iconMap[sprite.name] = sprite;
                Debug.Log($"✅ 載入圖示：{sprite.name}");
            }
        }
    }

    // 提供外部查詢圖示用
    public static Sprite GetSpriteById(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("❌ GetSpriteById：傳入的 id 為空");
            return null;
        }

        if (iconMap.TryGetValue(id, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            Debug.LogWarning($"❌ 找不到圖示：{id}");
            return null;
        }
    }
}
