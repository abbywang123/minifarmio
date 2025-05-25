// AuthInitializer.cs
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class AuthInitializer : MonoBehaviour
{
    async void Awake()
    {
        var opts = new InitializationOptions()
            .SetOption("projectId", "b96571f0-d087-4b8d-9058-59a6d804725b")
            .SetEnvironmentName("production");   // 你的唯一環境

        await UnityServices.InitializeAsync(opts);

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        Debug.Log($"✅ Sign-in OK  ▶  PlayerId: {AuthenticationService.Instance.PlayerId}");
    }
}
