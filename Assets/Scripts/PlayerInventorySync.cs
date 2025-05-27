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

    // ✅ 新增：購買道具的同步方法
    [ServerRpc(RequireOwnership = false)]
    public void BuyItemServerRpc(string itemId)
    {
        // 檢查是否已存在該物品，有的話疊加數量
        for (int i = 0; i < syncedInventory.Count; i++)
        {
            if (syncedInventory[i].itemId.ToString() == itemId)
            {
                var slot = syncedInventory[i];
                slot.count += 1;
                syncedInventory[i] = slot; // ✅ 替換更新
                Debug.Log($"🛒 增加背包項目：{itemId} → {slot.count}");
                return;
            }
        }

        // 否則新增新物品
        syncedInventory.Add(new SyncItemSlot
        {
            itemId = new FixedString32Bytes(itemId),
            count = 1
        });

        Debug.Log($"🛒 加入新道具：{itemId} x1");
    }
}

