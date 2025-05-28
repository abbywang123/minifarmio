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

    public TextMeshProUGUI waterLeftText;  // 剩餘澆水次數
    public Button backgroundButton;

    private Crop currentCrop;

    private int dailyWaterLimit = 15;
    private int waterLeft = 15;

    void Start()
    {
        waterButton.onClick.AddListener(WaterCrop);
        fertilizeButton.onClick.AddListener(FertilizeCrop);
        backgroundButton?.onClick.AddListener(Hide);

        Hide();
    }

    public void NewDayReset()
    {
        waterLeft = dailyWaterLimit;
        UpdateUI();
    }

    public void Show(Crop crop)
    {
        currentCrop = crop;
        gameObject.SetActive(true);
        UpdateUI();
    }

    public void Hide()
    {
        currentCrop = null;
        gameObject.SetActive(false);
    }

    void UpdateUI()
    {
        if (currentCrop == null) return;

        nameText.text = currentCrop.cropInfo.cropName;

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

    void FertilizeCrop()
    {
        if (currentCrop != null)
        {
            currentCrop.FertilizeCrop();
            UpdateUI();
        }
    }

    void OnEnable()
    {
        GameCalendar.OnNewDay += NewDayReset;
    }

    void OnDisable()
    {
        GameCalendar.OnNewDay -= NewDayReset;
    }

    void NewDayReset()
    {
        waterLeft = dailyWaterLimit;
        UpdateUI();
    }

}
