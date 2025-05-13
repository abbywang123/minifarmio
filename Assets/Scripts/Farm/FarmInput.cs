using UnityEngine;

public class FarmInput : MonoBehaviour
{
    public GameObject cropPrefab; // 指派 CropRuntime 預製物
    public CropData selectedCrop; // 指派目前選中的作物資料

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 滑鼠左鍵
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

            if (hit.collider != null && hit.collider.TryGetComponent<LandTile>(out var tile))
            {
                if (!tile.HasCrop())
                {
                    if (!tile.isTilled)
                        tile.Till();
                    else if (tile.CanPlant())
                        tile.Plant(selectedCrop, cropPrefab);
                }
                else if (tile.plantedCrop.IsMature())
                {
                    tile.Harvest();
                }
            }
        }
    }
}
