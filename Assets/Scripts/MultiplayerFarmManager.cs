using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;

public class MultiplayerFarmManager : NetworkBehaviour
{
    public GameObject tilePrefab;       // 指定 Tile 預製物（有 NetworkObject）
    public Transform gridParent;        // 要擺格子的容器（RectTransform）

    private Dictionary<Vector2Int, GameObject> tileMap = new();  // 可選：記錄格子方便查找

    public override async void OnNetworkSpawn()
    {
        if (IsHost)
        {
            await LoadAndSpawnFarm();
        }
    }

    private async Task LoadAndSpawnFarm()
    {
        Debug.Log("🟢 Host 嘗試載入 Cloud Save 農場資料...");

        FarmData farmData = null;

        try
        {
            farmData = await CloudSaveAPI.LoadFarmData();
            if (farmData == null || farmData.farmland == null)
            {
                Debug.LogWarning("⚠️ 無法載入農場資料或資料為空");
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ 載入農場失敗：" + e.Message);
            return;
        }

        Debug.Log($"✅ 成功載入 Cloud Save，生成 {farmData.farmland.Count} 塊田");

        foreach (var tileData in farmData.farmland)
        {
            GameObject tile = Instantiate(tilePrefab, gridParent);
            tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(tileData.x * 110, -tileData.y * 110);

            var tileNet = tile.GetComponent<TileNetworkSync>();
            tileNet.SetTile(tileData.x, tileData.y, tileData.cropId, tileData.growDays);

            tile.GetComponent<NetworkObject>().Spawn(true);  // ✅ 同步給所有 Client
            tileMap[new Vector2Int(tileData.x, tileData.y)] = tile;
        }
    }
}
