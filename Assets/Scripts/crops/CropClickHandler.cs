using UnityEngine;
using UnityEngine.EventSystems;

public class CropClickHandler : MonoBehaviour
{
    private Crop crop;

    void Awake()
    {
        crop = GetComponent<Crop>();
    }

    void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("❌ 點到 UI，不處理作物點擊");
            return;
        }

        if (crop != null && CropInfoPanel.Instance != null)
        {
            Debug.Log("🪴 點擊作物，打開作物資訊面板");
            CropInfoPanel.Instance.Show(crop);
        }
    }
}
