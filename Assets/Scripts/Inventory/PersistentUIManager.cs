using UnityEngine;

public class PersistentUIManager : MonoBehaviour
{
    public GameObject dragItemIconPrefab;
    private static bool initialized = false;

    void Awake()
    {
        if (initialized) return;

        // ✅ 僅初始化一次
        if (DragItemIcon.Instance == null && dragItemIconPrefab != null)
        {
            GameObject obj = Instantiate(dragItemIconPrefab);
            DontDestroyOnLoad(obj);
        }

        initialized = true;
    }
}
