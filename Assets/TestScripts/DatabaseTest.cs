using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;

public class DatabaseTest : MonoBehaviour
{
    void Start()
    {
        FirebaseDatabase.DefaultInstance
            .RootReference
            .Child("testWrite")
            .SetValueAsync("Hello Firebase!")
            .ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                    Debug.Log("✅ Firebase Database 寫入成功！");
                else
                    Debug.LogError($"❌ 寫入失敗：{task.Exception}");
            });
    }
}

