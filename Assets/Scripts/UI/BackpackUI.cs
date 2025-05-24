using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class BackpackUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;          // Player 的背包資料
    [SerializeField] private VisualTreeAsset slotUxml;     // slot.uxml 預製

    private List<VisualElement> uiSlots;
    private VisualElement grid;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        grid = root.Q<VisualElement>("grid");

        uiSlots = new List<VisualElement>();

        for (int i = 0; i < inventory.Slots.Count; i++)
        {
            var slot = slotUxml.CloneTree();          // 複製一個格子
            int index = i;
            slot.RegisterCallback<ClickEvent>(_ => OnSlotClick(index));
            grid.Add(slot);
            uiSlots.Add(slot);
        }

        Refresh();  // 初次顯示
    }

    void Refresh()
    {
        for (int i = 0; i < uiSlots.Count; i++)
        {
            var slotData = inventory.Slots[i];
            var icon = uiSlots[i].Q<VisualElement>("icon");
            var count = uiSlots[i].Q<Label>("count");

            if (slotData.IsEmpty)
            {
                icon.style.backgroundImage = null;
                count.text = "";
            }
            else
            {
                icon.style.backgroundImage = new StyleBackground(slotData.item.icon.texture);
                count.text = slotData.item.stackable ? slotData.count.ToString() : "";
            }
        }
    }

    void OnSlotClick(int index)
    {
        Debug.Log($"Clicked slot {index}");
        // 拖曳、使用物品等功能可在這裡擴充
    }
}
