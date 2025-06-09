using UnityEngine;
using System.Collections.Generic;

public static class IconDatabase
{
    private static Dictionary<string, Sprite> iconMap = new Dictionary<string, Sprite>();
    private static bool initialized = false;

    // 靜態建構子：第一次使用這個類別時執行
    static IconDatabase()
    {
        LoadAllIcons();
    }

    // ✅ 載入 Resources/Crops/picture 內所有圖片
    private static void LoadAllIcons()
    {
        iconMap.Clear(); // 清空舊資料避免重複載入
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Crops/picture");

        if (allSprites == null || allSprites.Length == 0)
        {
            Debug.LogWarning("⚠️ IconDatabase: 未載入任何圖示。請確認 Resources/Crops/picture 路徑與圖檔存在");
            return;
        }

        foreach (var sprite in allSprites)
        {
            if (string.IsNullOrEmpty(sprite.name)) continue;

            if (!iconMap.ContainsKey(sprite.name))
            {
                iconMap[sprite.name] = sprite;
                Debug.Log($"✅ 載入圖示：{sprite.name}");
            }
            else
            {
                Debug.LogWarning($"⚠️ 圖示名稱重複：{sprite.name}，已略過");
            }
        }

        initialized = true;
    }

    // ✅ 提供外部查詢圖示
    public static Sprite GetSpriteById(string id)
    {
        if (!initialized)
        {
            LoadAllIcons(); // fallback protection
        }

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

    // ✅ 額外提供：手動重新載入圖示（例如圖示更新後）
    public static void ReloadIcons()
    {
        LoadAllIcons();
        Debug.Log("🔄 圖示資料已重新載入");
    }
}

