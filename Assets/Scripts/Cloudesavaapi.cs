using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

public class CloudSaveAPI
{
    static readonly string EnvId = "production";

    // 🔧 建立 REST API URL
    static string BuildEndpoint(string key = "") =>
        $"https://services.api.unity.com/cloudsave/v1/data/projects/" +
        $"{AuthManager.ProjectId}/environments/{EnvId}/players/{AuthManager.PlayerId}/items" +
        (string.IsNullOrEmpty(key) ? "" : $"/{key}");

    // ✅ 儲存 FarmData
    public static async Task SaveFarmData(FarmData data)
    {
        if (string.IsNullOrEmpty(AuthManager.ProjectId) ||
            string.IsNullOrEmpty(AuthManager.PlayerId) ||
            string.IsNullOrEmpty(AuthManager.AccessToken))
        {
            Debug.LogWarning("⚠️ 尚未登入完成，無法儲存！");
            return;
        }

        // 👉 PUT 用 JSON 格式（多筆 key）
        var payload = new DataWrapper(new List<Item> {
            new Item { key = "inventory", value = data }
        });

        string putJson = JsonUtility.ToJson(payload);
        string putUrl = BuildEndpoint();

        Debug.Log("📤 PUT JSON: " + putJson);
        Debug.Log("🌐 PUT URL: " + putUrl);

        bool success = await SendSaveRequest(putUrl, putJson, "PUT");

        // 👉 若 PUT 失敗，改用 POST（單筆 key）
        if (!success)
        {
            Debug.LogWarning("⚠️ PUT 失敗，嘗試用 POST 建立 key...");

            string postJson = $"{{\"value\":{JsonUtility.ToJson(data)}}}";
            string postUrl = BuildEndpoint("inventory");

            Debug.Log("📤 POST JSON: " + postJson);
            Debug.Log("🌐 POST URL: " + postUrl);

            success = await SendSaveRequest(postUrl, postJson, "POST");

            if (!success)
                Debug.LogError("❌ POST 建立 key 也失敗！");
        }
    }

    // ✅ 讀取 FarmData
    public static async Task<FarmData> LoadFarmData()
    {
        if (string.IsNullOrEmpty(AuthManager.ProjectId) ||
            string.IsNullOrEmpty(AuthManager.PlayerId) ||
            string.IsNullOrEmpty(AuthManager.AccessToken))
        {
            Debug.LogWarning("⚠️ 尚未登入完成，無法讀取！");
            return null;
        }

        string url = BuildEndpoint("inventory");
        Debug.Log("🌐 GET URL: " + url);

        using UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Authorization", $"Bearer {AuthManager.AccessToken}");

        var op = req.SendWebRequest();
        while (!op.isDone) await Task.Yield();

        if (req.result == UnityWebRequest.Result.Success)
        {
            string json = req.downloadHandler.text;
            Debug.Log("📥 回傳 JSON: " + json);

            if (!json.TrimStart().StartsWith("{"))
            {
                Debug.LogError("❌ 非預期回傳格式：" + json);
                return null;
            }

            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
            return wrapper.value;
        }
        else
        {
            Debug.LogError($"❌ GET 失敗: {req.responseCode}\n{req.downloadHandler.text}");
            return null;
        }
    }

    // ✅ 共用傳送函式
    private static async Task<bool> SendSaveRequest(string url, string json, string method)
    {
        using UnityWebRequest req = new UnityWebRequest(url, method);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Authorization", $"Bearer {AuthManager.AccessToken}");
        req.SetRequestHeader("Content-Type", "application/json");

        var op = req.SendWebRequest();
        while (!op.isDone) await Task.Yield();

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"✅ {method} 儲存成功");
            return true;
        }
        else
        {
            Debug.LogWarning($"❌ {method} 儲存失敗（{req.responseCode}）: {req.error}\n{req.downloadHandler.text}");
            return false;
        }
    }

    // 🧩 結構定義
    [System.Serializable] private class Item
    {
        public string key;
        public FarmData value;
    }

    [System.Serializable] private class DataWrapper
    {
        public List<Item> data;
        public DataWrapper(List<Item> list) { data = list; }
    }

    [System.Serializable] private class Wrapper
    {
        public string key;
        public FarmData value;
    }
}
