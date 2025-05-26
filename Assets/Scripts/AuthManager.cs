using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public static class AuthManager
{
    // ✅ Unity Dashboard 的 Project ID
    public static string ProjectId = "b96571f0-d087-4b8d-9058-59a6d804725b";
    public static string PlayerId;
    public static string AccessToken;

    private static bool isInitialized = false;

    // ✅ 保證初始化完成
    public static async Task EnsureInitialized()
    {
        if (isInitialized) return;

        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        PlayerId = AuthenticationService.Instance.PlayerId;
        AccessToken = AuthenticationService.Instance.AccessToken;
        isInitialized = true;

        Debug.Log($"✅ 登入成功: {PlayerId}");
        Debug.Log($"📌 使用 ProjectId: {ProjectId}");
    }
}


