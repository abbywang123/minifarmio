using UnityEngine;

public class CropInfoPanelManager : MonoBehaviour
{
    public static CropInfoPanelManager Instance;

    [SerializeField] private CropInfoPanel panelPrefab; // 你的 CropInfoPanel prefab
    private CropInfoPanel activePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// 顯示作物資訊面板，若面板不存在則建立
    /// </summary>
    /// <param name="crop">要顯示的作物</param>
    public void ShowPanel(Crop crop)
    {
        if (activePanel == null)
        {
            activePanel = Instantiate(panelPrefab, transform);
        }

        activePanel.Show(crop);
        activePanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隱藏作物資訊面板
    /// </summary>
    public void HidePanel()
    {
        if (activePanel != null)
        {
            activePanel.Hide();
            activePanel.gameObject.SetActive(false);
        }
    }
}
