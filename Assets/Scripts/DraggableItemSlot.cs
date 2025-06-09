using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemId;
    public Canvas canvas;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image image;

    private Transform originalParent;
    private Vector3 originalWorldPosition;
    private Transform dragLayer;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();

        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>();
            Debug.Log($"🖼️ 自動抓到 Canvas：{canvas?.name}");
        }

        GameObject layerObj = GameObject.Find("DragLayer");
        if (layerObj != null)
        {
            dragLayer = layerObj.transform;
            Debug.Log("✅ 找到 DragLayer");
        }
        else
        {
            Debug.LogWarning("❌ 沒找到 DragLayer");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"🟡 開始拖曳 itemId = {itemId}");

        originalParent = transform.parent;
        originalWorldPosition = rectTransform.position;

        if (dragLayer != null)
        {
            transform.SetParent(dragLayer, true);
            Debug.Log("📤 已移動到 DragLayer");
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.blocksRaycasts = false;
        }

        if (image != null)
        {
            image.raycastTarget = false;
        }

        // ✅ 記錄拖曳資料（跨場景傳遞）
        DragItemData.draggingItemId = itemId;
        DragItemData.draggingIcon = image?.sprite;

        // ✅ 顯示拖曳圖示
        if (DragItemIcon.Instance != null && DragItemData.draggingIcon != null)
        {
            DragItemIcon.Instance.Show(DragItemData.draggingIcon);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position += (Vector3)(eventData.delta / canvas.scaleFactor);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("🟢 結束拖曳");

        transform.SetParent(originalParent, true);
        rectTransform.position = originalWorldPosition;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        if (image != null)
        {
            image.raycastTarget = true;
        }

        // ✅ 結束後可選擇不馬上清除 DragItemData，等播種或使用時再清
        // DragItemData.Clear(); // 選擇性

        DragItemIcon.Instance?.Hide();
    }
}


