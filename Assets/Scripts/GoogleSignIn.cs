using UnityEngine;
using Firebase;
using Firebase.Auth;
using Google;
using System.Threading.Tasks;
using Google.SignIn;

public class FirebaseInit : MonoBehaviour
{
    private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    void Start()
    {
        // 初始化 Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase 初始化完成");
            }
            else
            {
                Debug.LogError("Firebase 初始化失敗: " + task.Result);
            }
        });

        // 設定 Google Sign-In（使用你自己的 Web client ID）
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = "你在Firebase專案設定中的Web client ID",
            RequestEmail = true,
            RequestIdToken = true
        };

        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.DefaultInstance.SignOut(); // 避免記住前次帳號
    }

    // ✅ 被按鈕呼叫的方法
    public void SignInWithGoogle()
    {
        Debug.Log("開始 Google 登入");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthFinished);
    }

    private void OnGoogleAuthFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsCanceled)
        {
            Debug.Log("Google 登入取消");
        }
        else if (task.IsFaulted)
        {
            Debug.LogError("Google 登入錯誤: " + task.Exception);
        }
        else
        {
            // 取得 ID token 後，使用 Firebase 登入
            var credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
            {
                if (authTask.IsCanceled || authTask.IsFaulted)
                {
                    Debug.LogError("Firebase 登入失敗: " + authTask.Exception);
                }
                else
                {
                    FirebaseUser user = authTask.Result;
                    Debug.Log("登入成功，歡迎 " + user.DisplayName);
                }
            });
        }
    }
}
