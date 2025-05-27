using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CloudSaveInitializer : MonoBehaviour
{
    async void Start()
    {
        while (!AuthenticationService.Instance.IsSignedIn)
            await Task.Yield();

        Debug.Log("🧩 正在初始化 Cloud Save...");

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(new Dictionary<string, object> {
                { "inventory", new Dictionary<string, object> {
                    { "playerName", "init" },
                    { "gold", 100 }
                }}
            });

            Debug.Log("✅ 初始化成功，可使用 REST API");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("❌ 初始化失敗！" + ex.Message);
        }
    }
}
