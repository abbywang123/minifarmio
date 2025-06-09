// ✅ 完整拖曳功能測試腳本
// 請掛在場景中任一 UI Image 上

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragTestIcon : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector3 originalPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();

        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        if (!canvas) Debug.LogError("❌ 找不到 Canvas！拖曳無效");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("🟡 OnBeginDrag 觸發");
        originalPos = rectTransform.position;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position += (Vector3)(eventData.delta / canvas.scaleFactor);
        Debug.Log($"➡️ 拖曳中：{rectTransform.position}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("🟢 拖曳結束");
        rectTransform.position = originalPos;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}

