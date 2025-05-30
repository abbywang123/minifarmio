using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("道具資訊")]
    public string itemId; // ✅ 拖曳的道具 ID

    [Header("UI 設定")]
    public Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("❌ [DraggableItemSlot] 無法自動取得 Canvas！");
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // ✅ 通知 InventoryManager：開始拖曳這個道具
        InventoryManager.Instance?.SetDraggingItem(itemId);
        Debug.Log($"🟡 開始拖曳道具：{itemId}");

        // ✅ 顯示滑鼠下的圖示
        if (InventoryManager.Instance.IconMap.TryGetValue(itemId, out var sprite))
        {
            DragItemIcon.Instance?.Show(sprite);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null)
        {
            Debug.LogWarning("❌ Canvas 尚未設定，拖曳取消");
            return;
        }

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // ✅ 隱藏滑鼠下的圖示
        DragItemIcon.Instance?.Hide();
    }
}
