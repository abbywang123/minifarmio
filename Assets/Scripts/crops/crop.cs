using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropInfo cropInfo;

    private float growthProgress = 0f;
    private float health = 100f;
    private float quality = 100f;

    private LandTile landTile;

    public float GetGrowthProgress() => growthProgress;
    public float GetHealth() => health;
    public float GetQuality() => quality;
    public CropInfo GetInfo() => cropInfo;
    public float GetGrowthProgressNormalized() => Mathf.Clamp01(growthProgress / 100f);
    public float GetHealthNormalized() => Mathf.Clamp01(health / 100f);
    public float GetQualityNormalized() => Mathf.Clamp01(quality / 100f);

    private float growthRate => cropInfo.growthRate;

    public void Init(CropInfo info, LandTile tile)
    {
        cropInfo = info;
        landTile = tile;
        growthProgress = 0f;
        health = 100f;
        quality = 100f;
    }

    public void UpdateGrowthAuto()
{
    var wm = WeatherManager.Instance;
    if (wm == null) return;

    float temperature = wm.temperature;
    string weather = wm.currentWeather;
    bool isNight = RealTimeDayNightSystem.Instance != null && RealTimeDayNightSystem.Instance.IsNight;

    float soilMoisture = landTile != null ? landTile.GetCurrentMoisture() : 0.5f;

    float optimalTemp = (cropInfo.suitableMinTemperature + cropInfo.suitableMaxTemperature) / 2f;
    float tempTolerance = (cropInfo.suitableMaxTemperature - cropInfo.suitableMinTemperature) / 2f;

    float optimalHumidity = (cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f;
    float humidityTolerance = (cropInfo.suitableMaxHumidity - cropInfo.suitableMinHumidity) / 2f;

    float tempFactor = Mathf.Clamp01(1 - Mathf.Abs(temperature - optimalTemp) / tempTolerance);
    float humidityFactor = Mathf.Clamp01(1 - Mathf.Abs(soilMoisture - optimalHumidity) / humidityTolerance);
    float weatherFactor = (weather == "Rain") ? 0.9f : 1f;

    growthProgress += growthRate * tempFactor * humidityFactor * weatherFactor;

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

    ApplySpecialEffect(weather, isNight);

    if (growthProgress >= 100f)
        Harvest();
}

    private void ApplySpecialEffect(string currentWeather, bool isNight)
    {
        var player = FindFirstObjectByType<Player>();
        if (player == null) return;

        var wallet = player.GetComponent<PlayerWallet>();
        var inventory = player.GetComponent<Inventory>();

        switch (cropInfo.specialEffect)
        {
            case SpecialEffectType.NightBoost:
                if (isNight)
                    growthProgress += growthRate * 0.5f;
                break;

            case SpecialEffectType.ExtraGoldOnHarvest:
                if (Random.value < 0.2f && wallet != null)
                {
                    wallet.Earn(10);
                    Debug.Log("🎉 你獲得額外金幣 +10！");
                }
                break;

            case SpecialEffectType.RainGrowthBoost:
                if (currentWeather == "Rain")
                    growthProgress += growthRate * 0.5f;
                break;

            case SpecialEffectType.AntiRot:
                health += 1f;
                break;

            case SpecialEffectType.ProduceAuraFertilizer:
                if (inventory != null)
                {
                    var fertilizer = ItemDatabase.I.Get("Fertilizer");
                    if (fertilizer != null)
                    {
                        inventory.Add(fertilizer, 1);
                        Debug.Log("✨ 你獲得通用肥料，已放入背包！");
                    }
                }
                break;

            case SpecialEffectType.StableYield:
                quality = Mathf.Clamp(quality, 80f, 100f);
                break;

            case SpecialEffectType.DroughtResistant:
                float avgHumidity = (cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f;
                if (avgHumidity < 0.3f)
                    growthProgress += growthRate * 0.4f;
                break;
        }
    }

    public void Harvest()
    {
        Debug.Log($"{cropInfo.cropName} 成熟並收成！");

        var player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogWarning("[Crop] 找不到 Player，無法收成。");
            return;
        }

        bool ok = player.GetComponent<Inventory>().Add(cropInfo.harvestItem, 1);

        if (ok)
        {
            Debug.Log($"已將 {cropInfo.cropName} 放入玩家背包。");
        }
        else
        {
            Debug.LogWarning($"玩家背包已滿，將 {cropInfo.cropName} 放入倉庫。");
            WarehouseManager.Instance.inventory.Add(cropInfo.harvestItem, 1);
        }

        Destroy(gameObject);
    }

    public bool IsMature() => growthProgress >= 100f;

    public void WaterCrop()
    {
        Debug.Log($"{cropInfo.cropName} 澆水！");
        growthProgress += growthRate * 0.2f;
        health = Mathf.Min(health + 5f, 100f);
    }

    public void FertilizeCrop()
    {
        var inventory = FindFirstObjectByType<Player>()?.GetComponent<Inventory>();
        var fertilizer = ItemDatabase.I.Get("Fertilizer");

        if (inventory != null && fertilizer != null && inventory.Remove(fertilizer, 1))
        {
            Debug.Log($"{cropInfo.cropName} 施肥成功！");
            quality = Mathf.Min(quality + 10f, 100f);
            growthProgress += growthRate * 0.1f;
        }
        else
        {
            Debug.Log("❌ 沒有肥料！");
        }
    }
}
