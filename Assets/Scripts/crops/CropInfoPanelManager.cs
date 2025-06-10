using UnityEngine;

public class CropInfoPanelManager : MonoBehaviour
{
    public static CropInfoPanelManager Instance;

    [SerializeField] private CropInfoPanel panelInScene; // 指定場景中的 CropInfoPanel（非 prefab）
    private CropInfoPanel activePanel;

    // 靜態欄位暫存最後顯示的作物，跨場景共用
    private static Crop cachedCrop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        activePanel = panelInScene;

        if (activePanel != null)
        {
            activePanel.gameObject.SetActive(false); // 起始隱藏

            // 如果有暫存作物，重新顯示面板
            if (cachedCrop != null)
            {
                activePanel.Show(cachedCrop);
                activePanel.gameObject.SetActive(true);
            }
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

        cachedCrop = crop; // 存起來，切場景後還能用
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
            cachedCrop = null; // 隱藏時清除暫存
        }
    }
}
