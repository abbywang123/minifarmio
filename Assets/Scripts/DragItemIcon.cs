using UnityEngine;
using UnityEngine.UI;

public class DragItemIcon : MonoBehaviour
{
    public static DragItemIcon Instance { get; private set; }

    private RectTransform iconTransform;
    private Image iconImage;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // ✅ 跨場景保留

        iconTransform = GetComponent<RectTransform>();
        iconImage = GetComponent<Image>();
        Hide();
    }

    void Update()
    {
        if (iconImage.enabled)
        {
            iconTransform.position = Input.mousePosition;
        }
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
