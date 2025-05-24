using UnityEngine;
using Firebase;
using Firebase.Auth;
using Google;  // 保留這行即可
using System;
using System.Threading.Tasks;

public class FirebaseInit : MonoBehaviour
{
    private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    private void Start()
    {
        // 1. 初始化 Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("✅ Firebase 初始化完成");
            }
            else
            {
                Debug.LogError($"❌ Firebase 初始化失敗: {task.Result}");
            }
        });

        // 2. 設定 Google Sign-In（請換成你 Firebase Console 中的 Web Client ID）
        configuration = new GoogleSignInConfiguration
        {
            WebClientId ="482754796623-4igmho92984ucubmhvglcqof8vkv62r4.apps.googleusercontent.com",
            RequestEmail = true,
            RequestIdToken = true
        };

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.DefaultInstance.SignOut(); // 避免記住前次帳號
    }

    // 3. UI 按鈕觸發的登入方法
    public void SignInWithGoogle()
    {
        Debug.Log("📲 開始 Google 登入流程");
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthFinished);
    }

    private void OnGoogleAuthFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            Debug.LogError("❌ Google 登入失敗：" + task.Exception);
            return;
        }

        string idToken = task.Result.IdToken;
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
        {
            if (authTask.IsCanceled)
            {
                Debug.LogError("❌ Firebase 認證被取消");
                return;
            }
            if (authTask.IsFaulted)
            {
                Debug.LogError("❌ Firebase 認證失敗：" + authTask.Exception);
                return;
            }

            FirebaseUser newUser = authTask.Result;
            Debug.Log($"✅ 登入成功！歡迎：{newUser.DisplayName} ({newUser.Email})");
        });
    }
}
