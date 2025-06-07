using UnityEngine;
using UnityEngine.Tilemaps;

public class LandTileClickReceiver : MonoBehaviour
{
    public GameObject cropPrefab; // 指向 SeedlingPrefab
    private Tilemap tilemap;

    void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0) && DragItemData.draggingItemId != null)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int cellPos = tilemap.WorldToCell(worldPos);

            if (tilemap.HasTile(cellPos))
            {
                Vector3 cellWorld = tilemap.GetCellCenterWorld(cellPos);

                // 種下作物
                GameObject crop = Instantiate(cropPrefab, cellWorld, Quaternion.identity);
                var seed = crop.GetComponent<CropSeedling>();
                if (seed != null)
                {
                    seed.SetCrop(DragItemData.draggingItemId);
                }

                // 清除拖曳狀態
                DragItemData.draggingItemId = null;
                DragItemIcon.Instance.Hide();
            }
        }
    }
}
