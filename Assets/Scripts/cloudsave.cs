using UnityEngine;
using Unity.Services.CloudSave;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CloudSaveBootstrap : MonoBehaviour
{
    async void Start()
    {
        // ✅ 等待 AuthManager 登入完成
        while (!AuthenticationService.Instance.IsSignedIn)
            await Task.Yield();

        Debug.Log($"🧾 Player 已登入：{AuthenticationService.Instance.PlayerId}");

        // ✅ 準備資料
        var data = new Dictionary<string, object>
        {
            { "inventory", new Dictionary<string, object>
                {
                    { "playerName", "SDK初始化" },
                    { "gold", 999 }
                }
            }
        };

        // ✅ 使用 Cloud Save SDK 寫入
        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            Debug.Log("✅ Cloud Save 寫入成功！key: inventory");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Cloud Save 寫入失敗：{e.Message}");
        }
    }
}
