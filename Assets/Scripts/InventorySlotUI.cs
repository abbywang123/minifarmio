using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

        // 🔧 確保 CanvasGroup 存在
        canvasGroup = iconImage.GetComponent<CanvasGroup>();
        if (!canvasGroup)
            canvasGroup = iconImage.gameObject.AddComponent<CanvasGroup>();

        // 🧭 尋找 Canvas（可為 World 或 Screen Space）
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

        // 🧩 尋找 DragLayer 來放置拖曳物件
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

    /// <summary>
    /// 設定道具圖示與數量
    /// </summary>
    public void Setup(string id, int count)
    {
        itemId = id;
        iconImage.sprite = ItemDatabase.Instance.GetIcon(itemId);
        countText.text = count > 0 ? $"x{count}" : "";
    }

    /// <summary>
    /// 支援直接傳入 ItemSlot 結構
    /// </summary>
    public void Setup(ItemSlot slot)
    {
        Setup(slot.itemId, slot.count);
    }

    /// <summary>
    /// 啟用拖曳事件
    /// </summary>
    public void EnableDragging()
    {
        var trigger = iconImage.gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new List<EventTrigger.Entry>();

        // 🔹 Begin Drag
        Add(trigger, EventTriggerType.BeginDrag, (data) =>
        {
            originalParent = rectTransform.parent;
            originalPosition = rectTransform.position;

            rectTransform.SetParent(dragLayer, true);
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;

            InventoryManager.Instance.SetDraggingItem(itemId);
            DragItemData.draggingItemId = itemId;

            var icon = ItemDatabase.Instance.GetIcon(itemId);
            if (icon != null && DragItemIcon.Instance != null)
            {
                DragItemIcon.Instance.Show(icon);
            }
            else
            {
                Debug.LogWarning("⚠️ 無法顯示拖曳圖示");
            }

            Debug.Log($"🟡 開始拖曳 {itemId}");
        });

        // 🔸 Drag 中
        Add(trigger, EventTriggerType.Drag, (data) =>
        {
            PointerEventData eventData = (PointerEventData)data;
            if (canvas == null) return;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                canvas.GetComponent<RectTransform>(),
                eventData.position,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out Vector3 worldPos))
            {
                rectTransform.position = worldPos;
            }
        });

        // 🔹 End Drag
        Add(trigger, EventTriggerType.EndDrag, (data) =>
        {
            rectTransform.SetParent(originalParent, true);
            rectTransform.position = originalPosition;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            Debug.Log($"🟢 結束拖曳 {itemId}");
        });
    }

    /// <summary>
    /// 快速註冊事件
    /// </summary>
    private void Add(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }
}


