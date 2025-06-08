using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class LandTileMapping
{
    public Tilemap tilemap;
    public GameObject landTilePrefab;
}

public class LandTileSpawner : MonoBehaviour
{
    [Header("田地圖層")]
    public Tilemap[] tilemaps;

    [Header("生成設定")]
    public LandTileMapping[] mappings; // 👈 建立對應關係
    public Transform parent;

    void Start()
    {
        foreach (var map in mappings)
        {
            if (map.tilemap == null || map.landTilePrefab == null)
            {
                Debug.LogWarning("❗ 有未設定的 Tilemap 或 Prefab，請檢查 LandTileSpawner");
                continue;
            }

            foreach (var pos in map.tilemap.cellBounds.allPositionsWithin)
            {
               if (!map.tilemap.HasTile(pos)) continue;

                Vector3 worldPos = map.tilemap.GetCellCenterWorld(pos);
                Instantiate(map.landTilePrefab, worldPos, Quaternion.identity, parent);
            }
        }
    }
}
