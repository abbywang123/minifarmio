using UnityEngine;
using UnityEngine.UI;

public class DragItemIcon : MonoBehaviour
{
    public static DragItemIcon Instance { get; private set; }

    private RectTransform iconTransform;
    private Image iconImage;
    private Canvas canvas;

    void Awake()
    {
        // 單例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // ✅ 自動尋找並掛到 Canvas 下（不管是 Overlay 或 World）
        canvas = FindObjectOfType<Canvas>();
        if (canvas != null && transform.parent != canvas.transform && !IsChildOf(canvas.transform))
        {
            transform.SetParent(canvas.transform, false);
            Debug.Log("✅ DragItemIcon 已掛回 Canvas：" + canvas.name);
        }
        else if (canvas == null)
        {
            Debug.LogError("❌ 找不到 Canvas！");
        }

        // 抓 UI 組件
        iconTransform = GetComponent<RectTransform>();
        iconImage = GetComponent<Image>();

        if (iconImage == null)
        {
            Debug.LogError("❌ DragItemIcon 上缺少 Image 組件！");
            return;
        }

        if (iconTransform == null)
        {
            Debug.LogError("❌ DragItemIcon 上缺少 RectTransform！");
            return;
        }

        // ✅ 保持原比例、不變形
        iconImage.preserveAspect = true;

        // ✅ 設定預設大小
        iconTransform.sizeDelta = new Vector2(2, 2);

        Hide();
    }

    void Update()
    {
        if (iconImage != null && iconImage.enabled && canvas != null)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                iconTransform.position = Input.mousePosition;
            }
            else
            {
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.transform as RectTransform,
                    Input.mousePosition,
                    canvas.worldCamera,
                    out pos
                );
                iconTransform.localPosition = pos;
            }
        }
    }

    public void Show(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("⚠️ 嘗試顯示空圖示");
            return;
        }

        iconImage.sprite = sprite;
        iconImage.enabled = true;
        Debug.Log($"✅ Show 被呼叫，圖示名稱：{sprite.name}");
    }

    public void Hide()
    {
        if (iconImage != null)
        {
            iconImage.enabled = false;
            iconImage.sprite = null;
        }
    }

    private bool IsChildOf(Transform parent)
    {
        Transform current = transform.parent;
        while (current != null)
        {
            if (current == parent) return true;
            current = current.parent;
        }
        return false;
    }
}

