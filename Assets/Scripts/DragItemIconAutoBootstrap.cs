using UnityEngine;
using UnityEngine.UI;

public static class DragItemIconAutoBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (DragItemIcon.Instance != null) return;

        // 🟢 建立獨立 Canvas（不依賴場景）
        GameObject canvasGO = new GameObject("DragIconCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;  // 永遠顯示在最上層

        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        Object.DontDestroyOnLoad(canvasGO);

        // 🟢 建立圖示
        GameObject iconGO = new GameObject("DragItemIcon");
        iconGO.transform.SetParent(canvasGO.transform, false);
        iconGO.layer = LayerMask.NameToLayer("UI");

        RectTransform rt = iconGO.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(64, 64);

        Image img = iconGO.AddComponent<Image>();
        img.raycastTarget = false;

        DragItemIcon icon = iconGO.AddComponent<DragItemIcon>();
        icon.Init(img);

        Debug.Log("✅ [AutoBootstrap] DragItemIcon + Canvas 建立成功");
    }
}

