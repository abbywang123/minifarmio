using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileClickManager : MonoBehaviour
{
    public Tilemap tilemap;           // 拖入你的 Tilemap，例如 AllLandTile
    public GameObject cropPrefab;     // 拖入 SeedlingPrefab

    void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Debug.Log("🖱 滑鼠左鍵釋放");

            // 🔍 檢查是否點到 UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("❌ 點到 UI，不處理播種");
                return;
            }

            // 🎯 取得點擊座標對應的 tilemap cell
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
            cellPos.z = 0; // ✅ Tilemap 是 2D 平面，z 軸要為 0 才能正確判斷
            Debug.Log($"🧭 點擊座標：{mouseWorldPos} 對應 cell: {cellPos}");

            tilemap.SetColor(cellPos, Color.red);

            // 檢查點擊位置是否有 Tile
            if (!tilemap.HasTile(cellPos))
            {
                Debug.Log("❌ 點擊位置沒有 Tile，不播種");
                return;
            }

            // 📦 取得正在拖曳的種子 ID
            string seedId = DragItemData.draggingItemId;
            if (string.IsNullOrEmpty(seedId))
            {
                Debug.Log("⚠️ 沒有拖曳中的種子");
                return;
            }

            Debug.Log($"🌱 準備播種種子 ID：{seedId}");

            // ✅ Instantiate 作物
            Vector3 cellCenter = tilemap.GetCellCenterWorld(cellPos);
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

            // ✅ 清除拖曳狀態
            DragItemData.draggingItemId = null;
            if (DragItemIcon.Instance != null)
            {
                DragItemIcon.Instance.Hide();
                Debug.Log("🧼 已隱藏拖曳圖示");
            }
            else
            {
                Debug.LogWarning("❗ DragItemIcon.Instance 為 null");
            }

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.ClearDraggingItem();
                Debug.Log("🧹 已清除 Inventory 拖曳狀態");
            }
            else
            {
                Debug.LogWarning("❗ InventoryManager.Instance 為 null");
            }
        }
    }
}

