using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI Reference")]
    public Image iconImage;
    public TMP_Text countText;
    public Canvas canvas;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Transform dragLayer;
    private string itemId;

    void Awake()
    {
        rectTransform = iconImage.GetComponent<RectTransform>();

        // ⛑️ 加 CanvasGroup 確保可調透明與阻擋 Raycast
        canvasGroup = iconImage.GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = iconImage.gameObject.AddComponent<CanvasGroup>();

        // ✅ 自動取得 canvas
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        // ✅ 自動尋找 DragLayer
        var dl = GameObject.Find("DragLayer");
        if (dl != null)
        {
            dragLayer = dl.transform;
            Debug.Log("✅ InventorySlotUI 找到 DragLayer");
        }
        else
        {
            Debug.LogWarning("❌ DragLayer 不存在，拖曳可能無效");
        }
    }

    // 設定圖示與數量
    public void Setup(Sprite icon, string id, int count)
    {
        iconImage.sprite = icon;
        countText.text = count > 0 ? $"x{count}" : "";
        itemId = id;
    }

    // 啟用拖曳邏輯
    public void EnableDragging()
    {
        EventTrigger trigger = iconImage.gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();

        Add(trigger, EventTriggerType.BeginDrag, (data) =>
        {
            originalParent = rectTransform.parent;
            originalPosition = rectTransform.position;

            // 將格子提到畫面最上層顯示
            rectTransform.SetParent(dragLayer, true);
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;

            // 記錄拖曳資料
            InventoryManager.Instance.SetDraggingItem(itemId);
            DragItemData.draggingItemId = itemId;

            // 顯示滑鼠跟隨圖示
            if (InventoryManager.Instance.IconMap.TryGetValue(itemId, out var sprite))
            {
                DragItemIcon.Instance.Show(sprite);
            }

            Debug.Log($"🟡 開始拖曳 {itemId}");
        });

        Add(trigger, EventTriggerType.Drag, (data) =>
        {
            PointerEventData eventData = (PointerEventData)data;

            Vector3 worldPos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out worldPos))
            {
                rectTransform.position = worldPos;
            }
        });

        Add(trigger, EventTriggerType.EndDrag, (data) =>
        {
            // 恢復原本位置與層級
            rectTransform.SetParent(originalParent, true);
            rectTransform.position = originalPosition;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            // ⚠️ 若不跨場景可清除狀態，跨場景請保留拖曳資料
            // InventoryManager.Instance.ClearDraggingItem();
            // DragItemData.draggingItemId = null;
            // DragItemIcon.Instance.Hide();

            Debug.Log($"🟢 結束拖曳 {itemId}");
        });
    }

    private void Add(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }
}
