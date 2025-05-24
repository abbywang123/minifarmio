// Inventory.cs
using System.Collections.Generic;   // List<>
using UnityEngine;                  // MonoBehaviour、SerializeField

[System.Serializable]
public struct Slot
{
    public ItemData item;
    public int      count;

    public bool IsEmpty => item == null;
    public bool IsFull  => !IsEmpty && count >= item.maxStack;
}

public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 24;
    public List<Slot> Slots { get; private set; }

    void Awake()
    {
        Slots = new List<Slot>(capacity);
        for (int i = 0; i < capacity; i++)
            Slots.Add(new Slot());
    }

    /// 嘗試加入物品：先疊加已有 → 再找空格
    public bool Add(ItemData data, int amount = 1)
    {
        // 1. 先疊加到已存在且未滿的欄位
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].item == data && !Slots[i].IsFull)
            {
                Slot slot = Slots[i]; // 複製出來再修改
                int space = data.maxStack - slot.count;
                int add   = Mathf.Min(space, amount);
                slot.count += add;
                Slots[i] = slot; // 設回去
                amount -= add;
                if (amount == 0) return true;
            }
        }

        // 2. 再找空欄位塞入
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].IsEmpty)
            {
                int add = Mathf.Min(data.maxStack, amount);
                Slots[i] = new Slot { item = data, count = add };
                amount  -= add;
                if (amount == 0) return true;
            }
        }

        return false; // 無法完全放入（背包滿）
    }

    /// 嘗試移除物品：從任意欄位扣數量直到足夠為止
    public bool Remove(ItemData data, int amount = 1)
    {
        int total = 0;

        // 先確認是否足夠扣
        foreach (var slot in Slots)
        {
            if (slot.item == data)
                total += slot.count;
        }

        if (total < amount)
            return false; // 數量不夠

        // 逐格扣除
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].item == data && Slots[i].count > 0)
            {
                Slot slot = Slots[i]; // 拿出來再改
                int remove = Mathf.Min(slot.count, amount);
                slot.count -= remove;
                amount -= remove;

                if (slot.count == 0)
                    slot = new Slot(); // 清空

                Slots[i] = slot; // 設回去

                if (amount == 0)
                    return true;
            }
        }

        return true;
    }
}
