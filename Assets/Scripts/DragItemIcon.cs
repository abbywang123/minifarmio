using UnityEngine;
using UnityEngine.UI;

public class DragItemIcon : MonoBehaviour
{
    public static DragItemIcon Instance { get; private set; }

    private RectTransform iconTransform;
    private Image iconImage;

    public void Init(Image image)
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        iconImage = image;
        iconTransform = GetComponent<RectTransform>();

        iconImage.preserveAspect = true;
        iconTransform.sizeDelta = new Vector2(64, 64);

        Hide();
    }

    void Update()
    {
        if (!iconImage.enabled) return;

        // ✅ 只更新滑鼠位置，不再改變 parent（避免跨場景失效）
        iconTransform.position = Input.mousePosition;
    }

    public void Show(Sprite sprite)
    {
        if (sprite == null) return;
        iconImage.sprite = sprite;
        iconImage.enabled = true;
    }

    public void Hide()
    {
        iconImage.enabled = false;
        iconImage.sprite = null;
    }
}
