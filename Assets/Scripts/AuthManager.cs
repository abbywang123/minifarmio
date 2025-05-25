using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class AuthManager : MonoBehaviour
{
    // ✅ 手動設定 ProjectId（來自 Unity Dashboard）
    public static string ProjectId = "b96571f0-d087-4b8d-9058-59a6d804725b";
    public static string PlayerId;
    public static string AccessToken;

    async void Awake()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        PlayerId = AuthenticationService.Instance.PlayerId;
        AccessToken = AuthenticationService.Instance.AccessToken;

        Debug.Log($"✅ 登入成功: {PlayerId}");
        Debug.Log($"📌 使用 ProjectId: {ProjectId}");
        Debug.Log($"🎯 Project ID（AuthManager.ProjectId）: {ProjectId}");
        Debug.Log($"🎯 UnityServices.ProjectId（內部）: {Unity.Services.Core.UnityServices.State}"); // ← 非同步狀態

    }
}

