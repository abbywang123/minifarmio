using UnityEngine;

public class CropInfoPanelManager : MonoBehaviour
{
    public static CropInfoPanelManager Instance;

    [SerializeField] private CropInfoPanel panelPrefab;
    private CropInfoPanel activePanel;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowPanel(Crop crop)
    {
        if (activePanel == null)
        {
            activePanel = Instantiate(panelPrefab, transform);
        }

        activePanel.Show(crop);
    }

    public void HidePanel()
    {
        if (activePanel != null)
        {
            activePanel.Hide();
        }
    }
}
