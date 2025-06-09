using UnityEngine;
using System.Collections;

public class Crop : MonoBehaviour
{
    [SerializeField] private CropInfo cropInfo;

    private float growthProgress = 0f;
    private float health = 100f;
    private float quality = 100f;

    private SpriteRenderer spriteRenderer;
    private int currentStage = -1;
    private Sprite[] growthStages;
    private LandTile landTile;
    private Player cachedPlayer;

    private float growthRate => cropInfo.growthRate;

    public CropInfo Info => cropInfo;  // 公開屬性，外部透過這讀取 cropInfo

    public float GetGrowthProgress() => growthProgress;
    public float GetHealth() => health;
    public float GetQuality() => quality;

    public float GetGrowthProgressNormalized() => Mathf.Clamp01(growthProgress / 100f);
    public float GetHealthNormalized() => Mathf.Clamp01(health / 100f);
    public float GetQualityNormalized() => Mathf.Clamp01(quality / 100f);

    public bool IsMature() => growthProgress >= 100f;

    private void Awake()
    {
        spriteRenderer = transform.Find("SpriteObject")?.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("❌ 沒有找到 SpriteRenderer，請檢查 SpriteObject 是否存在且掛載正確！");

        cachedPlayer = FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        Debug.Log("Crop Start() Called");
        if (cropInfo != null)
        {
            Debug.Log($"✅ Crop Start 初始化：{cropInfo.cropName}");

            if (cropInfo.growthStages.Length > 0)
            {
                spriteRenderer.sprite = cropInfo.growthStages[0];
            }
        }
        else
        {
            Debug.LogWarning("⚠️ CropInfo 尚未初始化，請檢查是否有呼叫 Init()");
        }
    }

    private void OnEnable()
    {
        GameCalendar.OnNewDay += HandleNewDay;
    }

    private void OnDisable()
    {
        GameCalendar.OnNewDay -= HandleNewDay;
    }

    public void Init(CropInfo info, LandTile tile)
    {
        Debug.Log("Crop Init Called");
        cropInfo = info;
        landTile = tile;
        growthProgress = 0f;
        health = 100f;
        quality = 100f;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        Sprite icon = CropIconDatabase.GetSpriteById(info.seedId);
        if (icon != null)
        {
            spriteRenderer.sprite = icon;
            Debug.Log($"🌱 顯示作物圖示：{info.seedId}");
        }
        else
        {
            Debug.LogWarning($"❌ 找不到作物圖示：{info.seedId}");
        }

        LoadGrowthSprites();
        currentStage = -1;
        UpdateVisual();
        Debug.Log($"✅ Init 完成：{info.cropName}，初始值 {growthProgress}%");
    }

    private void LoadGrowthSprites()
    {
        int totalStages = 3;
        growthStages = new Sprite[totalStages];

        for (int i = 0; i < totalStages; i++)
        {
            string path = $"CropAnimator/{cropInfo.cropName}_{i}";
            growthStages[i] = Resources.Load<Sprite>(path);
            if (growthStages[i] == null)
            {
                Debug.LogWarning($"❌ 找不到成長圖：{path}");
            }
            else
            {
                Debug.Log($"✅ 成功載入：{path}");
            }
        }
    }

    public void UpdateGrowthAuto()
    {
        if (cachedPlayer == null) cachedPlayer = FindFirstObjectByType<Player>();
        var wm = WeatherManager.Instance;
        if (wm == null) return;

        float soilMoisture = landTile != null ? landTile.GetCurrentMoisture() : 0.5f;

        float temperature = wm.temperature;
        string weather = wm.currentWeather;
        bool isNight = RealTimeDayNightSystem.Instance != null && RealTimeDayNightSystem.Instance.IsNight;

        UpdateGrowth(temperature, soilMoisture, weather, isNight);
    }

    private void UpdateVisual()
    {
        if (growthStages == null || growthStages.Length == 0) return;
        
        int stage = Mathf.FloorToInt(GetGrowthProgressNormalized() * (growthStages.Length - 1));

        Debug.Log($"growthProgress: {growthProgress}, stage: {stage}, currentStage: {currentStage}");
        
        if (stage != currentStage)
        {
            currentStage = stage;
            spriteRenderer.sprite = growthStages[stage];
            StartCoroutine(AnimateGrow());
        }
    }

    private IEnumerator AnimateGrow()
    {
        Debug.Log($"🌿 播放生長動畫階段 {currentStage}！");
        Vector3 startScale = Vector3.one * 0.8f;
        Vector3 targetScale = Vector3.one;
        float duration = 0.3f;
        float elapsed = 0f;

        spriteRenderer.transform.localScale = startScale;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            spriteRenderer.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.transform.localScale = targetScale;
    }

    private void HandleNewDay()
    {
        Debug.Log("🗓 作物收到新的一天事件！");
        UpdateGrowthAuto(); // 使用實際天氣系統與土地濕度
    }

    public void UpdateGrowth(float temperature, float humidity, string weather, bool isNight)
    {
        float optimalTemp = (cropInfo.suitableMinTemperature + cropInfo.suitableMaxTemperature) / 2f;
        float optimalHumidity = (cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f;

        float tempTolerance = (cropInfo.suitableMaxTemperature - cropInfo.suitableMinTemperature) / 2f;
        float humidityTolerance = (cropInfo.suitableMaxHumidity - cropInfo.suitableMinHumidity) / 2f;

        float tempFactor = Mathf.Clamp01(1 - Mathf.Abs(temperature - optimalTemp) / tempTolerance);
        float humidityFactor = Mathf.Clamp01(1 - Mathf.Abs(humidity - optimalHumidity) / humidityTolerance);
        float weatherFactor = (weather == "Rain") ? 0.9f : 1f;

        growthProgress = Mathf.Clamp(growthProgress + growthRate * tempFactor * humidityFactor * weatherFactor, 0f, 100f);

        if (humidity < cropInfo.suitableMinHumidity)
        {
            health -= 5f;
            Debug.Log($"{cropInfo.cropName} 因濕度過低受損！");
        }

        if (weather == "Rain" && landTile != null)
        {
            landTile.AddMoisture(0.1f);
        }

        if (weather == "Rain" && Random.value < 0.2f)
            health -= 10f;

        health = Mathf.Clamp(health, 0f, 100f);

        ApplySpecialEffect(weather, isNight);

        if (growthProgress >= 100f)
            Harvest();

        UpdateVisual();
    }

    private void ApplySpecialEffect(string currentWeather, bool isNight)
    {
        if (cachedPlayer == null) return;

        var wallet = cachedPlayer.GetComponent<PlayerWallet>();
        var inventory = cachedPlayer.GetComponent<Inventory>();

        switch (cropInfo.specialEffect)
        {
            case SpecialEffectType.NightBoost:
                if (isNight)
                    growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.5f, 0f, 100f);
                break;

            case SpecialEffectType.ExtraGoldOnHarvest:
                if (wallet != null)
                    wallet.Earn(10);  // 使用 Earn 代替不存在的 AddMoney
                break;

            case SpecialEffectType.RainGrowthBoost:
                if (currentWeather == "Rain")
                    growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.5f, 0f, 100f);
                break;

            case SpecialEffectType.AntiRot:
                health = Mathf.Clamp(health + 1f, 0f, 100f);
                break;

            case SpecialEffectType.ProduceAuraFertilizer:
                if (inventory != null)
                {
                    var fertilizer = ItemDatabase.Get("Fertilizer");
                    if (fertilizer != null)
                        inventory.Add(fertilizer, 1);
                }
                break;

            case SpecialEffectType.DroughtResistant:
                float avgHumidity = (cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f;
                if (avgHumidity < 0.3f)
                    growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.4f, 0f, 100f);
                break;
        }
    }

    public void Harvest()
    {
        if (cachedPlayer == null)
        {
            Debug.LogWarning("[Crop] 找不到 Player，無法收成。");
            return;
        }

        var inventory = cachedPlayer.GetComponent<Inventory>();
        bool ok = inventory.Add(cropInfo.harvestItem, 1);

        if (ok)
        {
            WarehouseManager.Instance.inventory.Add(cropInfo.harvestItem, 1);
        }

        Destroy(gameObject);
    }

    public void WaterCrop()
    {
        growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.2f, 0f, 100f);
        health = Mathf.Min(health + 5f, 100f);
        UpdateVisual();
    }

    public void FertilizeCrop()
    {
        var inventory = cachedPlayer?.GetComponent<Inventory>();
        var fertilizer = ItemDatabase.I.Get("Fertilizer");

        if (inventory != null && fertilizer != null && inventory.Remove(fertilizer, 1))
        {
            quality = Mathf.Min(quality + 10f, 100f);
            growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.1f, 0f, 100f);
        }
        else
        {
            Debug.Log("❌ 沒有肥料！");
        }

        UpdateVisual();
    }
}
