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
        Debug.Log("🛠️ Awake: 初始化");

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

        // 🟡 視覺效果
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.5f;
            canvasGroup.blocksRaycasts = false;
        }

        if (image != null)
        {
            image.raycastTarget = false; // 讓目標可以接收 drop
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position += (Vector3)(eventData.delta / canvas.scaleFactor);
        Debug.Log($"➡️ 拖曳中... 當前位置：{rectTransform.position}");
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
    }
}


