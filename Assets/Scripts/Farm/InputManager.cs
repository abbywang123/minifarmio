using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject cropPrefab;
    [SerializeField] private SeedSelectorPanel seedPanel;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = cam.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);  // ✅ 修正：不再使用 out

            if (hit.collider != null)
            {
                var tile = hit.collider.GetComponent<LandTile>();
                if (tile == null) return;

                if (!tile.isTilled)
                {
                    tile.Till();                 // 翻土
                    return;
                }

                if (tile.CanPlant())
                {
                    seedPanel.Open(tile, cropPrefab); // 彈出選種子面板
                }
                else if (tile.HasCrop() && tile.plantedCrop.IsMature())
                {
                    tile.Harvest();
                    Debug.Log("收成完成");
                }
            }
        }
    }
}
