using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class CloudSaveBootstrap : MonoBehaviour
{
    private bool isSigningIn = false;

    async void Start()
    {
        Debug.Log("🔧 初始化 Unity Services...");
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn && !isSigningIn)
        {
            try
            {
                isSigningIn = true;
                Debug.Log("🔐 嘗試匿名登入...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log($"✅ 登入成功！PlayerId: {AuthenticationService.Instance.PlayerId}");
            }
            catch (AuthenticationException ex)
            {
                Debug.LogError("❌ 登入失敗：" + ex.Message);
            }
            finally
            {
                isSigningIn = false;
            }
        }
        else if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("✅ 已經登入，略過重複登入");
        }
    }
}
