using UnityEngine;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Cloud Save 資料讀寫輔助類別，使用 callback 傳回結果。
/// </summary>
public static class CloudSaveHelper
{
    private const string InventoryKey = "inventory";

    /// <summary>
    /// ✅ 儲存 FarmData 到雲端（轉為 JSON 格式）
    /// </summary>
    public static async Task SaveFarmData(FarmData data)
    {
        string json = JsonUtility.ToJson(data);

        var dict = new Dictionary<string, object>
        {
            { InventoryKey, json }
        };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(dict);
            Debug.Log("✅ [SDK] 儲存成功：inventory");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ 儲存失敗：{ex.Message}");
        }
    }

    /// <summary>
    /// ✅ 從雲端讀取 FarmData，讀取完成後透過 callback 傳回
    /// </summary>
    public static async Task LoadFarmData(System.Action<FarmData> onLoaded)
    {
        try
        {
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { InventoryKey });

            if (result.TryGetValue(InventoryKey, out var item))
            {
                string json = item.Value.GetAsString();
                FarmData data = JsonUtility.FromJson<FarmData>(json);
                Debug.Log("✅ [SDK] 讀取成功！");
                onLoaded?.Invoke(data);
            }
            else
            {
                Debug.LogWarning("⚠️ [SDK] 找不到 key: inventory");
                onLoaded?.Invoke(null);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ 讀取失敗：{ex.Message}");
            onLoaded?.Invoke(null);
        }
    }
}
