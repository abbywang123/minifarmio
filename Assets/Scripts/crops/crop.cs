using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

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

    public CropInfo Info => cropInfo;

    public float GetGrowthProgress() => growthProgress;
    public float GetHealth() => health;
    public float GetQuality() => quality;

    public float GetGrowthProgressNormalized() => Mathf.Clamp01(growthProgress / 100f);
    public float GetHealthNormalized() => Mathf.Clamp01(health / 100f);
    public float GetQualityNormalized() => Mathf.Clamp01(quality / 100f);

    public bool IsMature() => growthProgress >= 100f;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("❌ 沒有找到 SpriteRenderer，請檢查子物件是否掛載 SpriteRenderer！");

        cachedPlayer = FindFirstObjectByType<Player>();
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
        cropInfo = info;
        landTile = tile;
        growthProgress = 0f;
        health = 100f;
        quality = 100f;

        if (spriteRenderer == null)
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
        Debug.Log($"✅ Init 完成：{info.cropName}");
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
                Debug.LogWarning($"❌ 找不到成長圖：{path}");
            else
                Debug.Log($"✅ 成功載入：{path}");
        }
    }

    private void HandleNewDay()
    {
        Debug.Log("🗓 作物收到新的一天事件！");
        UpdateGrowthAuto();
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
        {
            health -= 10f;
            Debug.Log("⚡️ 暴雨導致作物受損！");
        }

        health = Mathf.Clamp(health, 0f, 100f);

        ApplySpecialEffect(weather, isNight);

        if (growthProgress >= 100f)
        {
            Harvest();
        }

        UpdateVisual();
    }

    private async void ApplySpecialEffect(string weather, bool isNight)
    {
        if (cachedPlayer == null) return;

        var inventoryManager = InventoryManager.Instance;

        switch (cropInfo.specialEffect)
        {
            case SpecialEffectType.NightBoost:
                if (isNight)
                    growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.5f, 0f, 100f);
                break;

            case SpecialEffectType.RainGrowthBoost:
                if (weather == "Rain")
                    growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.5f, 0f, 100f);
                break;

            case SpecialEffectType.AntiRot:
                health = Mathf.Clamp(health + 1f, 0f, 100f);
                break;

            case SpecialEffectType.ProduceAuraFertilizer:
            if (inventoryManager != null)
            {
                await inventoryManager.AddItemAsync("muck", 1);
                Debug.Log("🎉 特殊效果：獲得 1 個 muck");
            }
            break;

            case SpecialEffectType.DroughtResistant:
                float avgHumidity = (cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f;
                if (avgHumidity < 0.3f)
                    growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.4f, 0f, 100f);
                break;
        }
    }

    private void UpdateVisual()
    {
        if (growthStages == null || growthStages.Length == 0)
        {
            Debug.LogWarning("⚠️ growthStages 為空，無法更新圖！");
            return;
        }

        int stage = Mathf.FloorToInt(GetGrowthProgressNormalized() * (growthStages.Length - 1));
        Debug.Log($"UpdateVisual() - progress: {growthProgress}, stage: {stage}, currentStage: {currentStage}");

        if (growthStages[stage] == null)
            Debug.LogWarning($"⚠️ growthStages[{stage}] 是 null");
        else
            Debug.Log($"🎨 設定 sprite：{growthStages[stage].name}");

        if (spriteRenderer.sprite == null)
            Debug.LogWarning("🚨 spriteRenderer.sprite 是 null！");

        if (stage != currentStage)
        {
            currentStage = stage;
            spriteRenderer.sprite = growthStages[stage];
            Debug.Log($"✅ 圖已更新為階段 {stage}，圖名稱：{spriteRenderer.sprite?.name}");
            StartCoroutine(AnimateGrow());
        }
        else
        {
            Debug.Log("🔁 階段未改變，不換圖。");
        }
    }

    private IEnumerator AnimateGrow()
    {
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

    public async Task Harvest()
    {
        if (cachedPlayer == null)
        {
            Debug.LogWarning("[Crop] 找不到 Player，無法收成。");
            return;
        }

        var inventoryManager = InventoryManager.Instance;
        var wallet = cachedPlayer.GetComponent<PlayerWallet>();

        if (inventoryManager != null)
        {
            // 用 InventoryManager 的 AddItemAsync
            await inventoryManager.AddItemAsync(cropInfo.harvestItem.id, 1);

            if (cropInfo.specialEffect == SpecialEffectType.ExtraGoldOnHarvest && wallet != null)
                wallet.Earn(10);
        }

        landTile?.Harvest();
    }



    public void WaterCrop()
    {
        growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.2f, 0f, 100f);
        health = Mathf.Min(health + 5f, 100f);
        UpdateVisual();
    }

    public async void FertilizeCrop()
    {
        var inventoryManager = InventoryManager.Instance;
        if (inventoryManager == null) return;

        bool removed = await inventoryManager.RemoveItemAsync("muck", 1);
        if (removed)
        {
            quality = Mathf.Min(quality + 10f,100f);
            growthProgress = Mathf.Min(growthProgress + 5f, 100f);
            Debug.Log("🌿 已施肥，品質 +10，成長 +5");
        }
        else
        {
            Debug.LogWarning("❌ 背包中無 muck，施肥失敗");
        }
    }

} 
