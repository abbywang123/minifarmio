using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using TMPro;

public class MultiplayerFarmManager : NetworkBehaviour
{
    public GameObject tilePrefab;       // 指定 Tile 預製物
    public Transform gridParent;        // 要擺格子的容器（空物件）

    private int gridWidth = 5;
    private int gridHeight = 4;

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            LoadAndSpawnFarm();
        }
    }

    void LoadAndSpawnFarm()
    {
        Debug.Log("🟢 Host 載入農場資料並生成");

        // 這裡用假資料，之後可改成 CloudSaveAPI.LoadFarmData()
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Vector2 pos = new Vector2(x * 110, -y * 110); // 間距調整
                GameObject tile = Instantiate(tilePrefab, gridParent);
                tile.GetComponent<RectTransform>().anchoredPosition = pos;

                var tileNet = tile.GetComponent<TileNetworkSync>();
                tileNet.SetTile(x, y, "wheat", 3); // 預設每格都有 wheat

                tile.GetComponent<NetworkObject>().Spawn(true); // ✅ Host 同步給 Client
            }
        }
    }
}
