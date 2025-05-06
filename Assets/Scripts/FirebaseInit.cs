using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement;

public class FirebaseInit : MonoBehaviour
{
    public static FirebaseAuth Auth { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);   // 保持跨場景存在
#if UNITY_EDITOR
        Application.runInBackground = true;   // 方便桌機測試
#endif
    }

    async void Start()
    {
        var status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status == DependencyStatus.Available)
        {
            Debug.Log("✅ Firebase ready");
            Auth = FirebaseAuth.DefaultInstance;
            // 自動帶入已登入帳號（遊戲重新進入時）
            if (Auth.CurrentUser != null)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
        else
        {
            Debug.LogError($"❌ Firebase init error: {status}");
        }
    }
}
