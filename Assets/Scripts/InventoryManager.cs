using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject slotPrefab;      // InventorySlot prefab
    public Transform gridParent;       // 放格子的容器（GridParent）

    [Header("Icon Resources")]
    public Sprite defaultIcon;         // 預設圖示
    public Sprite turnipIcon;
    public Sprite carrotIcon;

    private Dictionary<string, Sprite> iconMap;

    void Start()
    {
        // ✅ 使用現有的 ItemSlot 類（不要再定義一次）
        List<ItemSlot> inventoryData = new()
        {
            new ItemSlot { itemId = "turnip", count = 3 },
            new ItemSlot { itemId = "carrot", count = 5 },
            new ItemSlot { itemId = "turnip", count = 2 },
        };

        // 建立 itemId → Sprite 的對照表
        iconMap = new Dictionary<string, Sprite>
        {
            { "turnip", turnipIcon },
            { "carrot", carrotIcon }
        };

        // 依據資料動態生成格子
        foreach (var slot in inventoryData)
        {
            GameObject go = Instantiate(slotPrefab, gridParent);
            go.name = $"Slot_{slot.itemId}";

            // 設定圖示
            Transform iconTf = go.transform.Find("Icon");
            if (iconTf != null)
            {
                Image iconImage = iconTf.GetComponent<Image>();
                iconImage.sprite = iconMap.ContainsKey(slot.itemId) ? iconMap[slot.itemId] : defaultIcon;
            }

            // 設定數量
            Transform countTf = go.transform.Find("CountText");
            if (countTf != null)
            {
                TMP_Text countText = countTf.GetComponent<TMP_Text>();
                countText.text = $"x{slot.count}";
            }
        }
    }
}

