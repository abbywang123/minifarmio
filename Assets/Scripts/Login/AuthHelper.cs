using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using System.Threading.Tasks;

public static class AuthHelper
{
    private static bool isSigningIn = false;
    private static bool hasSignedIn = false;

    public static string ProjectId = "b96571f0-d087-4b8d-9058-59a6d804725b"; // ← 你的 Unity Dashboard 專案 ID


    public static string PlayerId => AuthenticationService.Instance.PlayerId;
    public static string AccessToken => AuthenticationService.Instance.AccessToken;

    public static async Task EnsureSignedIn()
    {
        if (hasSignedIn) return;

        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            Debug.Log("🔧 初始化 UnityServices...");
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn && !isSigningIn)
        {
            try
            {
                isSigningIn = true;
                Debug.Log("🔐 嘗試匿名登入...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"✅ 登入成功：{AuthenticationService.Instance.PlayerId}");
                hasSignedIn = true;
            }
            catch (AuthenticationException ex)
            {
                Debug.LogError("❌ 登入失敗：" + ex.Message);
                throw;
            }
            finally
            {
                isSigningIn = false;
            }
        }
        else if (AuthenticationService.Instance.IsSignedIn)
        {
            hasSignedIn = true;
            Debug.Log("✅ 已登入，略過重複登入");
        }
    }
}

