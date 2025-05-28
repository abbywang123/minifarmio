using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class TileClickManager : MonoBehaviour
{
    private PlayerControls controls;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Click.performed += ctx => StartCoroutine(DelayedClick());
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    IEnumerator DelayedClick()
    {
        yield return null; // 等待一幀，讓 EventSystem 更新狀態

        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("👆 點到 UI，不處理地圖點擊");
            yield break;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            TileNetworkSync tile = hit.collider.GetComponent<TileNetworkSync>();
            if (tile != null)
            {
                tile.PlantCropServerRpc("carrot");
                Debug.Log("🌱 種植紅蘿蔔！");
            }
        }
    }
}


