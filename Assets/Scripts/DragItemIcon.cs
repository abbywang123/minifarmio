using UnityEngine;
using UnityEngine.UI;

public class DragItemIcon : MonoBehaviour
{
    public static DragItemIcon Instance { get; private set; }

    private RectTransform iconTransform;
    private Image iconImage;

    void Awake()
    {
        // 確保只有一個實例存在（單例）
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // ✅ 跨場景保留

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

        Hide();
    }

    void Update()
    {
        // 將圖示跟著滑鼠移動
        if (iconImage.enabled)
        {
            iconTransform.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// 顯示拖曳圖示
    /// </summary>
    public void Show(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogWarning("⚠️ 嘗試顯示空圖示");
            return;
        }

        iconImage.sprite = sprite;
        iconImage.enabled = true;
    }

    /// <summary>
    /// 隱藏拖曳圖示
    /// </summary>
    public void Hide()
    {
        iconImage.enabled = false;
        iconImage.sprite = null;
    }
}
