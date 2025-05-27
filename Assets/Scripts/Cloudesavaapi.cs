using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class CloudSaveAPI
{
    // ✅ 儲存 FarmData（轉成 JSON 後存入）
    public static async Task SaveFarmData(FarmData data)
    {
        await AuthHelper.EnsureSignedIn();

        string json = JsonUtility.ToJson(data);
        var dict = new Dictionary<string, object>
        {
            { "inventory", json }
        };

        // ✅ 使用新方法（不再使用 ForceSaveAsync）
        await CloudSaveService.Instance.Data.Player.SaveAsync(dict);
        Debug.Log("✅ SDK 儲存成功！");
    }

    // ✅ 讀取 FarmData（還原 JSON）
    public static async Task<FarmData> LoadFarmData()
{
    await AuthHelper.EnsureSignedIn();

    var keys = new HashSet<string> { "inventory" };

    // ✅ 使用新版 Player.LoadAsync
    var result = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

    if (result.TryGetValue("inventory", out var entry))
    {
        // ✅ 使用新版 Value.GetAsString() 取得 JSON 字串
        string json = entry.Value.GetAsString();
        FarmData farmData = JsonUtility.FromJson<FarmData>(json);
        Debug.Log("✅ SDK 讀取成功！");
        return farmData;
    }
    else
    {
        Debug.LogWarning("⚠️ 沒有找到 inventory 資料");
        return null;
    }
}

}


