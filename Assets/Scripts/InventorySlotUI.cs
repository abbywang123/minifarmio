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

        canvasGroup = iconImage.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = iconImage.gameObject.AddComponent<CanvasGroup>();

        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();

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

    public void Setup(string id, int count)
    {
        itemId = id;
        iconImage.sprite = ItemDatabase.Instance.GetIcon(itemId);
        iconImage.enabled = iconImage.sprite != null;

        countText.text = count > 0 ? $"x{count}" : "";
    }

    public void Setup(ItemSlot slot)
    {
        Setup(slot.itemId, slot.count);
    }

    public void EnableDragging()
    {
        var trigger = iconImage.gameObject.AddComponent<EventTrigger>();
        trigger.triggers = new List<EventTrigger.Entry>();

        Add(trigger, EventTriggerType.BeginDrag, (data) =>
        {
            if (string.IsNullOrEmpty(itemId)) return;

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

            Debug.Log($"🟡 開始拖曳 {itemId}");
        });

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

        Add(trigger, EventTriggerType.EndDrag, (data) =>
        {
            rectTransform.SetParent(originalParent, true);
            rectTransform.position = originalPosition;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            Debug.Log($"🟢 結束拖曳 {itemId}");
        });

        // ✅ 點擊顯示 itemId（再點一次可關閉）
        Add(trigger, EventTriggerType.PointerClick, (data) =>
        {
            if (string.IsNullOrEmpty(itemId)) return;

            int count = 0;
            foreach (var slot in InventoryManager.Instance.GetInventoryData())
            {
                if (slot.itemId == itemId)
                {
                    count = slot.count;
                    break;
                }
            }

            InventoryManager.Instance.ShowItemInfo(itemId, count);
            Debug.Log($"🔍 點擊顯示 itemId：{itemId}");
        });
    }

    private void Add(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }
}




