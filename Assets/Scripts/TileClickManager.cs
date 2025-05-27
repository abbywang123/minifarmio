using UnityEngine;
using UnityEngine.InputSystem; // ✅ 注意是新版命名空間
using UnityEngine.EventSystems;

public class TileClickManager : MonoBehaviour
{
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Click.performed += ctx => OnClick();
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void OnClick()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TileNetworkSync tile = hit.collider.GetComponent<TileNetworkSync>();
            if (tile != null)
            {
                tile.PlantCropServerRpc("carrot");
            }
        }
    }
}

