using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CloudSaveInitializer : MonoBehaviour
{
    async void Start()
    {
        // 等待玩家登入
        while (!AuthenticationService.Instance.IsSignedIn)
            await Task.Yield();

        Debug.Log("🧩 正在初始化 Cloud Save...");

        try
        {
            // 🔸 建立初始 FarmData 物件
            FarmData initialData = new FarmData
            {
                playerName = "init",
                gold = 1000,
                maxInventorySize = 12,
                inventory = new List<ItemSlot>(),
                farmland = new List<FarmlandTile>()
            };

            // 🔸 將 FarmData 序列化成 JSON 字串
            string json = JsonUtility.ToJson(initialData);

            // 🔸 儲存進 Cloud Save 的 "inventory" 欄位
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object>
            {
                { "inventory", json }
            });

            Debug.Log("✅ 初始化成功，可使用 REST API");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ 初始化失敗！" + ex.Message);
        }
    }
}
