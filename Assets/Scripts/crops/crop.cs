using UnityEngine;
using System.Collections;

public class Crop : MonoBehaviour
{
    private float growthProgress = 0f;
    private float health = 100f;
    private float quality = 100f;

    private SpriteRenderer spriteRenderer;
    private int currentStage = -1;
    private Sprite[] growthStages;
    private LandTile landTile;

    private Player cachedPlayer;

    public CropInfo cropInfo;

    // Getter 方法
    public float GetGrowthProgress() => growthProgress;
    public float GetHealth() => health;
    public float GetQuality() => quality;

    public CropInfo GetInfo() => cropInfo;
    public float GetGrowthProgressNormalized() => Mathf.Clamp01(growthProgress / 100f);
    public float GetHealthNormalized() => Mathf.Clamp01(health / 100f);
    public float GetQualityNormalized() => Mathf.Clamp01(quality / 100f);

    private float growthRate => cropInfo != null ? cropInfo.growthRate : 0f;

    private void Awake()
    {
        // 抓取 SpriteObject 子物件上的 SpriteRenderer
        spriteRenderer = transform.Find("SpriteObject")?.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            Debug.LogError("❌ 沒有找到 SpriteRenderer，請檢查 SpriteObject 是否存在且掛載正確！");

        // 載入生長階段的圖片
        LoadGrowthSprites();

        cachedPlayer = FindFirstObjectByType<Player>();
    }

    private void Start()
    {
        if (cropInfo != null && cropInfo.growthStages.Length > 0)
        {
            spriteRenderer.sprite = cropInfo.growthStages[0];
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
        cropInfo = info;
        landTile = tile;
        growthProgress = 0f;
        health = 100f;
        quality = 100f;

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 用 CropIconDatabase 設定初始圖示
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
        currentStage = -1; // 重設階段
        UpdateVisual();
    }

    private void LoadGrowthSprites()
    {
        if (cropInfo == null)
        {
            Debug.LogWarning("CropInfo 為空，無法載入生長階段圖片");
            return;
        }

        growthStages = new Sprite[cropInfo.growthStages.Length];
        for (int i = 0; i < cropInfo.growthStages.Length; i++)
        {
            growthStages[i] = cropInfo.growthStages[i];
        }
    }

    public void UpdateGrowthAuto()
    {
        if (growthStages == null || growthStages.Length == 0)
            return;

        int stage = Mathf.FloorToInt(GetGrowthProgressNormalized() * (growthStages.Length - 1));
        if (stage != currentStage)
        {
            currentStage = stage;
            spriteRenderer.sprite = growthStages[stage];
            StartCoroutine(AnimateGrow());
        }
    }

    private IEnumerator AnimateGrow()
    {
        Debug.Log($"🌱 播放動畫！成長階段: {currentStage}");
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
        // TODO: 根據實際天氣系統傳入真實數據
        float defaultTemp = (cropInfo.suitableMinTemperature + cropInfo.suitableMaxTemperature) / 2f;
        float defaultHumidity = (cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f;
        UpdateGrowth(defaultTemp, defaultHumidity, "Sunny", false);
    }

    public void UpdateGrowth(float temperature, float humidity, string weather, bool isNight)
    {
        if (cropInfo == null) return;

        float optimalTemp = (cropInfo.suitableMinTemperature + cropInfo.suitableMaxTemperature) / 2f;
        float tempTolerance = (cropInfo.suitableMaxTemperature - cropInfo.suitableMinTemperature) / 2f;

        float optimalHumidity = (cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f;
        float humidityTolerance = (cropInfo.suitableMaxHumidity - cropInfo.suitableMinHumidity) / 2f;

        float soilMoisture = landTile != null ? landTile.GetCurrentMoisture() : optimalHumidity;

        float tempFactor = Mathf.Clamp01(1 - Mathf.Abs(temperature - optimalTemp) / tempTolerance);
        float humidityFactor = Mathf.Clamp01(1 - Mathf.Abs(soilMoisture - optimalHumidity) / humidityTolerance);
        float weatherFactor = (weather == "Rain") ? 0.9f : 1f;

        growthProgress = Mathf.Clamp(growthProgress + growthRate * tempFactor * humidityFactor * weatherFactor, 0f, 100f);

        if (soilMoisture < cropInfo.suitableMinHumidity)
        {
            health -= 5f;
            Debug.Log($"{cropInfo.cropName} 因土壤乾燥受損！");
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
                {
                    growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.5f, 0f, 100f);
                }
                break;

            case SpecialEffectType.ExtraGoldOnHarvest:
                // 這邊你可自行補充細節
                break;

            case SpecialEffectType.RainGrowthBoost:
                if (currentWeather == "Rain")
                {
                    growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.5f, 0f, 100f);
                }
                break;

            case SpecialEffectType.AntiRot:
                health = Mathf.Clamp(health + 1f, 0f, 100f);
                break;

            case SpecialEffectType.ProduceAuraFertilizer:
                if (inventory != null)
                {
                    var fertilizer = ItemDatabase.I.Get("Fertilizer"); // 通用肥料
                    if (fertilizer != null)
                    {
                        inventory.Add(fertilizer, 1);
                        Debug.Log("✨ 你獲得通用肥料，已放入背包！");
                    }
                    else
                    {
                        Debug.LogWarning("❌ 找不到通用肥料物品資料！");
                    }
                }
                break;

            case SpecialEffectType.DroughtResistant:
                float avgHumidity = (cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f;
                if (avgHumidity < 0.3f)
                {
                    growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.4f, 0f, 100f);
                }
                break;
        }
    }

    public void Harvest()
    {
        Debug.Log($"{cropInfo.cropName} 成熟並收成！");

        if (cachedPlayer == null)
        {
            Debug.LogWarning("[Crop] 找不到 Player，無法收成。");
            return;
        }

        var inventory = cachedPlayer.GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogWarning("[Crop] Player 沒有 Inventory 組件。");
            return;
        }

        bool ok = inventory.Add(cropInfo.harvestItem, 1);

        if (ok)
        {
            Debug.Log($"收成成功：{cropInfo.harvestItem.itemName} x1");

            if (WarehouseManager.Instance != null)
            {
                WarehouseManager.Instance.inventory.Add(cropInfo.harvestItem, 1);
            }
        }
        else
        {
            Debug.LogWarning("背包空間不足，無法收成");
        }

        Destroy(gameObject);
    }

    public void WaterCrop()
    {
        growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.2f, 0f, 100f);
        health = Mathf.Clamp(health + 5f, 0f, 100f);
        Debug.Log($"{cropInfo.cropName} 澆水！");
        UpdateVisual();
    }

    public void FertilizeCrop()
    {
        if (cachedPlayer == null)
        {
            Debug.LogWarning("[Crop] 找不到 Player，無法施肥。");
            return;
        }

        var inventory = cachedPlayer.GetComponent<Inventory>();
        var fertilizer = ItemDatabase.I.Get("Fertilizer");

        if (inventory != null && fertilizer != null && inventory.Remove(fertilizer, 1))
        {
            quality = Mathf.Clamp(quality + 10f, 0f, 100f);
            growthProgress = Mathf.Clamp(growthProgress + growthRate * 0.1f, 0f, 100f);
            Debug.Log($"{cropInfo.cropName} 施肥成功！");
            UpdateVisual();
        }
        else
        {
            Debug.Log("❌ 沒有肥料或背包中沒有肥料！");
        }
    }

    private void UpdateVisual()
    {
        UpdateGrowthAuto();
        // 你可以這邊加上其它視覺更新的邏輯
    }
}
