using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryDTO
{
    public List<SlotDTO> slots;

    public InventoryDTO(List<Slot> original)
    {
        slots = new List<SlotDTO>();
        foreach (var slot in original)
        {
            slots.Add(new SlotDTO(slot));
        }
    }
}

[Serializable]
public class SlotDTO
{
    public string itemId;
    public int count;

    public SlotDTO(Slot slot)
    {
        itemId = slot.item?.id ?? "";
        count = slot.count;
    }
}
