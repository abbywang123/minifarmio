using UnityEngine;
using UnityEngine.InputSystem; // 引入新版 Input System

public class FarmInput : MonoBehaviour
{
    public GameObject cropPrefab;   // 指派 Crop 預製物
    public CropInfo selectedCrop;   // ✅ 改為 CropInfo（舊的 CropData 改掉）

    void Update()
    {
        // 判斷滑鼠左鍵是否剛按下（這一幀）
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 取得滑鼠世界座標
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // 射線檢查是否打到地板格
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.TryGetComponent<LandTile>(out var tile))
            {
                if (!tile.HasCrop())
                {
                    if (!tile.isTilled)
                        tile.Till();
                    else if (tile.CanPlant())
                        tile.Plant(selectedCrop, cropPrefab);  // ✅ 現在 selectedCrop 是 CropInfo
                }
                else if (tile.plantedCrop.IsMature())
                {
                    tile.Harvest();
                }
                else
                {
                    // 👉 顯示作物狀態面板
                    var panel = FindObjectOfType<CropInfoPanel>();
                    if (panel != null)
                        panel.Show(tile.plantedCrop);
                }
            }
        }
    }
}
