using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileClickManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap[] tilemaps; // 拖入 FarmLandTile、PondLandTile、SandLandTile 等

    [Header("作物設定")]
    public GameObject cropPrefab; // 拖入 SeedlingPrefab

    void Start()
    {
        foreach (var tilemap in tilemaps)
        {
            Debug.Log($"📋 掃描 tilemap: {tilemap.name}");

            BoundsInt bounds = tilemap.cellBounds;
            foreach (var pos in bounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos))
                {
                    Debug.Log($"📌 {tilemap.name} 上有 tile at cell: {pos}");
                }
            }
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Debug.Log("🖱 滑鼠左鍵釋放");

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("❌ 點到 UI，不處理播種");
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int cellPos = tilemaps[0].WorldToCell(mouseWorldPos);
            cellPos.z = 0;

            Debug.Log($"🧭 點擊座標：{mouseWorldPos} 對應 cell: {cellPos}");

            // 🔍 嘗試從多層 tilemap 找出有 tile 的那一層
            Tilemap targetTilemap = null;
            foreach (var tilemap in tilemaps)
            {
                if (tilemap.HasTile(cellPos))
                {
                    targetTilemap = tilemap;
                    Debug.Log($"✅ 播種目標 tilemap 為：{tilemap.name}");
                    break;
                }
            }

            if (targetTilemap == null)
            {
                Debug.Log("❌ 所有 tilemap 都沒有該格子，不播種");
                return;
            }

            // 🌱 準備播種
            string seedId = DragItemData.draggingItemId;
            if (string.IsNullOrEmpty(seedId))
            {
                Debug.Log("⚠️ 沒有拖曳中的種子");
                return;
            }

            Debug.Log($"🌱 準備播種種子 ID：{seedId}");

            Vector3 cellCenter = targetTilemap.GetCellCenterWorld(cellPos);
            GameObject crop = Instantiate(cropPrefab, cellCenter, Quaternion.identity);

            var seedling = crop.GetComponent<CropSeedling>();
            if (seedling == null)
            {
                Debug.LogError("❌ cropPrefab 上沒有 CropSeedling 腳本，請確認掛載正確");
            }
            else
            {
                seedling.SetCrop(seedId);
                Debug.Log("✅ 已設置 CropSeedling 圖示");
            }

            // 清除拖曳圖示與狀態
            DragItemData.draggingItemId = null;
            if (DragItemIcon.Instance != null)
            {
                DragItemIcon.Instance.Hide();
                Debug.Log("🧼 已隱藏拖曳圖示");
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ClearDraggingItem();
                Debug.Log("🧹 已清除 Inventory 拖曳狀態");
            }
        }
    }
}


