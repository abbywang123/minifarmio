using UnityEngine;
using UnityEngine.EventSystems;

public class TileClickManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 mousePos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                TileNetworkSync tile = hit.collider.GetComponent<TileNetworkSync>();
                if (tile != null)
                {
                    tile.PlantCropServerRpc("carrot"); // 播種作物
                }
            }
        }
    }
}

