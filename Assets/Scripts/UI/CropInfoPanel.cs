using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CropInfoPanel : MonoBehaviour
{
    [Header("UI 元素")]
    public TextMeshProUGUI nameText;

    public Slider growthSlider;
    public Slider healthSlider;
    public Slider qualitySlider;

    public TextMeshProUGUI growthText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI qualityText;

    public Button waterButton;
    public Button fertilizeButton;
    public Button harvestButton;

    public TextMeshProUGUI waterLeftText;
    public TextMeshProUGUI fertilizerText; // ✅ 新增：肥料顯示文字

    public Button closeButton; // ✅ 新增：關閉按鈕

    private Crop currentCrop;
    private int dailyWaterLimit = 15;
    private int waterLeft = 15;

    void Start()
    {
        waterButton.onClick.AddListener(WaterCrop);
        fertilizeButton.onClick.AddListener(FertilizeCrop);
        harvestButton.onClick.AddListener(HarvestCrop);
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        Hide();
    }

    public void Show(Crop crop)
    {
        Debug.Log("CropInfoPanel Show() called for crop: " + crop.Info.cropName);
        currentCrop = crop;
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        currentCrop = null;
        gameObject.SetActive(false);
    }

    public void NewDayReset()
    {
        waterLeft = dailyWaterLimit;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (currentCrop == null) return;

        nameText.text = currentCrop.Info.cropName;

        float g = currentCrop.GetGrowthProgressNormalized();
        float h = currentCrop.GetHealthNormalized();
        float q = currentCrop.GetQualityNormalized();

        growthSlider.value = g;
        healthSlider.value = h;
        qualitySlider.value = q;

        growthText.text = $"{Mathf.RoundToInt(g * 100)}%";
        healthText.text = $"{Mathf.RoundToInt(h * 100)}%";
        qualityText.text = $"{Mathf.RoundToInt(q * 100)}%";

        waterLeftText.text = $"剩餘澆水次數：{waterLeft}";
        waterButton.interactable = (waterLeft > 0);
        harvestButton.interactable = currentCrop.IsMature();

        // ✅ 更新肥料數量文字
        int fertilizerCount = GetFertilizerCount();
        if (fertilizerText != null)
            fertilizerText.text = $"肥料數量：{fertilizerCount}";

        // ✅ 沒肥料就停用施肥按鈕
        fertilizeButton.interactable = (fertilizerCount > 0);
    }

    void WaterCrop()
    {
        if (currentCrop != null && waterLeft > 0)
        {
            currentCrop.WaterCrop();
            waterLeft--;
            UpdateUI();
        }
    }

    async void FertilizeCrop()
    {
        if (currentCrop != null && GetFertilizerCount() > 0)
        {
            bool removed = await InventoryManager.Instance.RemoveItemAsync("fertilizer", 1);
            if (removed)
            {
                currentCrop.FertilizeCrop();
                UpdateUI();
            }
            else
            {
                Debug.Log("❌ 無法扣除肥料");
            }
        }
        else
        {
            Debug.Log("❌ 沒有肥料");
        }
    }

    void HarvestCrop()
    {
        if (currentCrop != null && currentCrop.IsMature())
        {
            currentCrop.Harvest();
            Hide();
        }
    }

    int GetFertilizerCount()
    {
        if (InventoryManager.Instance == null) return 0;

        var inventory = InventoryManager.Instance.GetInventoryData();
        var slot = inventory.Find(s => s.itemId == "fertilizer");
        return slot != null ? slot.count : 0;
    }

    void OnEnable()
    {
        GameCalendar.OnNewDay += NewDayReset;
    }

    void OnDisable()
    {
        GameCalendar.OnNewDay -= NewDayReset;
    }
}
