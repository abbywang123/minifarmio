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
    public TextMeshProUGUI fertilizerText; // muck 顯示用

    public Button closeButton;

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

    async void UpdateUI()
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

        // ✅ muck 數量顯示
        int muckCount = await GetMuckCountAsync();
        if (fertilizerText != null)
            fertilizerText.text = $"肥料數量：{muckCount}";

        fertilizeButton.interactable = (muckCount > 0);
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
        if (currentCrop == null) return;

        var inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Debug.LogWarning("❌ InventoryManager 尚未初始化");
            return;
        }

        int count = await GetMuckCountAsync();
        if (count <= 0)
        {
            Debug.Log("❌ 沒有 muck 可使用");
            return;
        }

        bool removed = await inventory.RemoveItemAsync("muck", 1);
        if (removed)
        {
            currentCrop.FertilizeCrop();
            UpdateUI();
        }
        else
        {
            Debug.Log("❌ 無法扣除 muck");
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

    async System.Threading.Tasks.Task<int> GetMuckCountAsync()
    {
        var inventory = InventoryManager.Instance;
        if (inventory == null) return 0;

        var list = inventory.GetInventoryData();
        var slot = list.Find(s => s.itemId == "muck");
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
