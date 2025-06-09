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

    public void SetCapacity(int newCap)
    {
        capacity = newCap;
        Slots = new List<Slot>(capacity);
        for (int i = 0; i < capacity; i++)
            Slots.Add(new Slot());
    }

    // ✅ 農場用 FromDTO：支援 Cloud Save 的 ItemSlot 結構
    public void FromDTO(List<ItemSlot> dtoSlots)
    {
        Slots = new List<Slot>(capacity);

        for (int i = 0; i < capacity; i++)
        {
            var slot = new Slot();

            if (i < dtoSlots.Count)
            {
                var itemData = ItemDatabase.Instance.GetItemData(dtoSlots[i].itemId);
                slot.item = itemData;
                slot.count = dtoSlots[i].count;
            }

            Slots.Add(slot);
        }
    }

    // ✅ 倉庫用 FromDTO：支援 JSON 存檔的 SlotDTO 結構
    public void FromDTO(List<SlotDTO> dtoList)
    {
        Slots = new List<Slot>(capacity);

        for (int i = 0; i < capacity; i++)
        {
            var slot = new Slot();

            if (i < dtoList.Count)
            {
                var itemData = ItemDatabase.Instance.GetItemData(dtoList[i].itemId);
                slot.item = itemData;
                slot.count = dtoList[i].count;
            }

            Slots.Add(slot);
        }
    }

    public Slot GetSlot(int index)
    {
        if (index < 0 || index >= Slots.Count) return new Slot();
        return Slots[index];
    }

    public bool Add(ItemData data, int amount = 1)
    {
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

        return false;
    }

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


