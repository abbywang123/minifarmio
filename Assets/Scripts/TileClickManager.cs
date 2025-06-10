using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileClickManager : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap[] tilemaps;

    [Header("作物設定")]
    public GameObject seedlingPrefab; // 播種時生成的作物預設物件

    void Update()
    {
        if (!Mouse.current.leftButton.wasReleasedThisFrame)
            return;

        Debug.Log("🖱 滑鼠左鍵釋放");

        // 點到 UI 則不處理
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("❌ 點到 UI，不處理播種");
            return;
        }

        // 把滑鼠位置轉為世界位置，再轉成 Tilemap Cell
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3Int cellPos = tilemaps[0].WorldToCell(mouseWorldPos);
        cellPos.z = 0;

        Debug.Log($"🧭 點擊座標：{mouseWorldPos} 對應 cell: {cellPos}");

        // 尋找有該格子的 Tilemap
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

        // 取得地格中心，然後找地上是否有 LandTile 物件
        Vector3 cellCenter = targetTilemap.GetCellCenterWorld(cellPos);
        Collider2D hit = Physics2D.OverlapPoint(cellCenter);
        LandTile tile = hit?.GetComponent<LandTile>();

        if (tile == null)
        {
            Debug.LogWarning("❌ 找不到 LandTile，無法完成操作！");
            return;
        }

        // 若該地已有作物 → 顯示資訊
        if (tile.HasCrop())
        {
            Debug.Log("🌾 該地已有作物，顯示作物資訊");
            if (CropInfoPanelManager.Instance == null)
            {
                Debug.LogError("❌ CropInfoPanelManager.Instance 是 null，請檢查單例初始化或物件是否存在");
                return;
            }
            if (tile.plantedCrop == null)
            {
                Debug.LogWarning("❌ tile.plantedCrop 是 null");
                return;
            }
            CropInfoPanelManager.Instance.HidePanel(); // 強制先關閉面板
            CropInfoPanelManager.Instance.ShowPanel(tile.plantedCrop);
            return;
        }


        // 如果尚未鋤地，點一下自動鋤地，不播種
        if (!tile.isTilled)
        {
            Debug.Log("🪓 尚未鋤地，自動鋤地中...");
            tile.Till();
            return;
        }

        // 播種流程開始
        string seedId = DragItemData.draggingItemId;
        if (string.IsNullOrEmpty(seedId))
        {
            Debug.Log("⚠️ 沒有拖曳中的種子");
            return;
        }

        Debug.Log($"🌱 播種種子 ID：{seedId}");

        // 從資料庫取得作物資訊
        CropInfo info = CropDatabase.GetCropBySeedId(seedId);
        if (info == null)
        {
            Debug.LogWarning($"❌ 無法從資料庫取得作物資訊：{seedId}");
            return;
        }

        // 在 tile 位置生成 Seedling，並設定資料
        GameObject seedling = Instantiate(seedlingPrefab, cellCenter, Quaternion.identity, tile.transform);
        var seedlingScript = seedling.GetComponent<CropSeedling>();

        if (seedlingScript == null)
        {
            Debug.LogError("❌ SeedlingPrefab 上沒有 CropSeedling 腳本！");
        }
        else
        {
            seedlingScript.SetCrop(seedId, tile);
            Debug.Log("✅ 已設置 CropSeedling");
        }

        // 清除拖曳狀態
        DragItemData.draggingItemId = null;
        DragItemIcon.Instance?.Hide();
        InventoryManager.Instance?.ClearDraggingItem();
    }
}
