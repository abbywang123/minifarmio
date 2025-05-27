using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Slot
{
    public ItemData item;
    public int count;

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

    // ✅ [新增] 設定容量 + 初始化 Slots
    public void SetCapacity(int newCap)
    {
        capacity = newCap;
        Slots = new List<Slot>(capacity);
        for (int i = 0; i < capacity; i++)
            Slots.Add(new Slot());
    }

    // ✅ [新增] 從 DTO 資料恢復背包內容
    public void FromDTO(InventoryDTO dto)
    {
        Slots = new List<Slot>(capacity);

        for (int i = 0; i < capacity; i++)
        {
            var slot = new Slot();

            if (i < dto.slots.Count)
            {
                var itemData = ItemDatabase.I.Get(dto.slots[i].itemId);
                slot.item = itemData;
                slot.count = dto.slots[i].count;
            }

            Slots.Add(slot);
        }
    }

    // ✅ [選配] 取得指定欄位（避免 UI index 越界）
    public Slot GetSlot(int index)
    {
        if (index < 0 || index >= Slots.Count) return new Slot();
        return Slots[index];
    }

    // ✅ 原本：加入物品
    public bool Add(ItemData data, int amount = 1)
    {
        // 1. 先疊加到已存在且未滿的欄位
        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].item == data && !Slots[i].IsFull)
            {
                Slot slot = Slots[i];
                int space = data.maxStack - slot.count;
                int add = Mathf.Min(space, amount);
                slot.count += add;
                Slots[i] = slot;
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
                amount -= add;
                if (amount == 0) return true;
            }
        }

        return false; // 無法完全放入（背包滿）
    }

    // ✅ 原本：移除物品
    public bool Remove(ItemData data, int amount = 1)
    {
        int total = 0;

        foreach (var slot in Slots)
        {
            if (slot.item == data)
                total += slot.count;
        }

        if (total < amount)
            return false;

        for (int i = 0; i < Slots.Count; i++)
        {
            if (Slots[i].item == data && Slots[i].count > 0)
            {
                Slot slot = Slots[i];
                int remove = Mathf.Min(slot.count, amount);
                slot.count -= remove;
                amount -= remove;

                if (slot.count == 0)
                    slot = new Slot();

                Slots[i] = slot;

                if (amount == 0)
                    return true;
            }
        }

        return true;
    }

        public int CountOf(ItemData data)
    {
        int total = 0;
        foreach (var slot in Slots)
        {
            if (slot.item == data)
                total += slot.count;
        }
        return total;
    }

}
