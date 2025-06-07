using UnityEngine;

public class CropClickHandler : MonoBehaviour
{
    private Crop crop;

    private void Awake()
    {
        crop = GetComponent<Crop>();
    }

    private void OnMouseDown()
    {
        if (crop != null && CropInfoPanelManager.Instance != null)
        {
            CropInfoPanelManager.Instance.ShowPanel(crop);
        }
    }
}
