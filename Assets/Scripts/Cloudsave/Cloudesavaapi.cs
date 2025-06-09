using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// 封裝 Unity Cloud Save 儲存與讀取農場資料的 API。
/// </summary>
public static class CloudSaveAPI
{
    private const string InventoryKey = "inventory";

    /// <summary>
    /// ✅ 將 FarmData 儲存到 Cloud Save（轉成 JSON）
    /// </summary>
    public static async Task SaveFarmData(FarmData data)
    {
        await AuthHelper.EnsureSignedIn();

        string json = JsonUtility.ToJson(data);

        var dict = new Dictionary<string, object>
        {
            { InventoryKey, json }
        };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(dict);
            Debug.Log("✅ Cloud Save 儲存成功！");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ 儲存失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// ✅ 從 Cloud Save 讀取 FarmData（還原 JSON）
    /// </summary>
    public static async Task<FarmData> LoadFarmData()
    {
        await AuthHelper.EnsureSignedIn();

        var keys = new HashSet<string> { InventoryKey };

        try
        {
            var result = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            if (result.TryGetValue(InventoryKey, out var entry))
            {
                string json = entry.Value.GetAsString();
                FarmData farmData = JsonUtility.FromJson<FarmData>(json);
                Debug.Log("✅ Cloud Save 讀取成功！");
                return farmData;
            }
            else
            {
                Debug.LogWarning("⚠️ Cloud Save 中沒有找到 inventory 資料");
                return null;
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ 讀取失敗: {ex.Message}");
            return null;
        }
    }
}



