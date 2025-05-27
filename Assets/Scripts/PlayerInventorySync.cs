using UnityEngine;
using Unity.Netcode;
using Unity.Collections; // FixedString32Bytes
using TMPro;
using System.Collections.Generic;
using System;

public struct SyncItemSlot : INetworkSerializable, IEquatable<SyncItemSlot>
{
    public FixedString32Bytes itemId;
    public int count;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref itemId);
        serializer.SerializeValue(ref count);
    }

    public bool Equals(SyncItemSlot other)
    {
        return itemId.Equals(other.itemId) && count == other.count;
    }

    public override string ToString() => $"{itemId} x{count}";
}

public class PlayerInventorySync : NetworkBehaviour
{
    public NetworkList<SyncItemSlot> syncedInventory;
    public TMP_Text debugText;

    private List<ItemSlot> localInventory = new();

    private void Awake()
    {
        syncedInventory = new NetworkList<SyncItemSlot>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LoadFromPlayerPrefs();
        }

        // ✅ 改成正確版本支援的寫法
        syncedInventory.OnListChanged += change =>
        {
            if (debugText != null)
                debugText.text = string.Join("\n", syncedInventory);
        };
    }

    private void LoadFromPlayerPrefs()
    {
        string json = PlayerPrefs.GetString("inventoryData", "");
        if (string.IsNullOrEmpty(json)) return;

        InventoryWrapper wrapper = JsonUtility.FromJson<InventoryWrapper>(json);
        localInventory = wrapper.inventory;

        foreach (var item in localInventory)
        {
            syncedInventory.Add(new SyncItemSlot
            {
                itemId = item.itemId,
                count = item.count
            });
        }
    }
}
