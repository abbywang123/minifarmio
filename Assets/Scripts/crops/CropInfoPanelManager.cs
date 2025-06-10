using UnityEngine;



public class CropInfoPanelManager : MonoBehaviour
{
    public static CropInfoPanelManager Instance;

    [SerializeField] private CropInfoPanel panelInScene; // 👈 指定場景中的 CropInfoPanel（非 prefab）
    private CropInfoPanel activePanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // ✅ 使用場景中的面板
        activePanel = panelInScene;

        if (activePanel != null)
        {
            activePanel.gameObject.SetActive(false); // 起始隱藏
        }
        else
        {
            Debug.LogError("❌ panelInScene 尚未在 Inspector 指定！");
        }
    }

    public void ShowPanel(Crop crop)
    {
        if (activePanel == null)
        {
            Debug.LogError("❌ 沒有可用的 CropInfoPanel，請檢查是否有設好引用");
            return;
        }

        activePanel.Show(crop);
        activePanel.gameObject.SetActive(true);
        Debug.Log("✅ 顯示作物資訊面板");
    }

    public void HidePanel()
    {
        if (activePanel != null)
        {
            activePanel.Hide();
            activePanel.gameObject.SetActive(false);
        }
    }
}
