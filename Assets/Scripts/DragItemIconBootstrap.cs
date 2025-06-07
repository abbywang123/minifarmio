using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 開機時動態產生 DragItemIcon，並用 DontDestroyOnLoad 保存
/// </summary>
public class DragItemIconBootstrap : MonoBehaviour
{
    void Awake()
    {
        // 若已存在，就不再建立
        if (DragItemIcon.Instance != null) { Destroy(gameObject); return; }

        // ① 建立一個 root GameObject
        GameObject go = new GameObject("DragItemIcon");
        go.transform.SetParent(null);            // 保證是場景根層
        go.layer = LayerMask.NameToLayer("UI");  // 建議放到 UI Layer

        // ② 加必需元件
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(64, 64);      // 圖示大小

        Image img = go.AddComponent<Image>();
        img.raycastTarget = false;               // 不檔 Raycast

        // ③ 加上 DragItemIcon 腳本並初始化
        DragItemIcon icon = go.AddComponent<DragItemIcon>();
        icon.Init(img);

        // ④ 常駐
        DontDestroyOnLoad(go);
        Debug.Log("✅ DragItemIcon 建立並永久保留");
    }
}
