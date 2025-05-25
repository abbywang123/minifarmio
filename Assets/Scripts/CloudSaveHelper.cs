using UnityEngine;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class CloudSaveHelper
{
    // ✅ 儲存資料（用 JsonUtility 將 FarmData 轉成 JSON 字串）
    public static async Task SaveFarmData(FarmData data)
    {
        string json = JsonUtility.ToJson(data);

        await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
        {
            { "inventory", json }
        });

        Debug.Log("✅ [SDK] 儲存成功：inventory");
    }

    // ✅ 讀取資料並回傳給 callback
    public static async Task LoadFarmData(System.Action<FarmData> onLoaded)
    {
        var result = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "inventory" });

        if (result.TryGetValue("inventory", out var item))
{
    string json = item.Value.GetAsString();  // ✅ 從 Item.Value 拿出 JSON 字串
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
}
