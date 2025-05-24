using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropInfo cropInfo;

    private float growthProgress = 0f;
    private float health = 100f;
    private float quality = 100f;
    private float growthRate => cropInfo.growthRate;

    public void UpdateGrowth(float temperature, float humidity, string weather, bool isNight)
    {
        float optimalTemp = (cropInfo.suitableMinTemperature + cropInfo.suitableMaxTemperature) / 2f;
        float tempTolerance = (cropInfo.suitableMaxTemperature - cropInfo.suitableMinTemperature) / 2f;

        float optimalHumidity = (cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f;
        float humidityTolerance = (cropInfo.suitableMaxHumidity - cropInfo.suitableMinHumidity) / 2f;

        float tempFactor = Mathf.Clamp01(1 - Mathf.Abs(temperature - optimalTemp) / tempTolerance);
        float humidityFactor = Mathf.Clamp01(1 - Mathf.Abs(humidity - optimalHumidity) / humidityTolerance);
        float weatherFactor = (weather == "Rain") ? 0.9f : 1f;

        growthProgress += cropInfo.growthRate * tempFactor * humidityFactor * weatherFactor;

        if (weather == "Rain" && Random.value < 0.2f)
            health -= 10f;

        ApplySpecialEffect(weather, isNight);

        if (growthProgress >= 100f)
            Harvest();
    }

    private void ApplySpecialEffect(string currentWeather, bool isNight)
    {
        switch (cropInfo.specialEffect)
        {
            case SpecialEffectType.NightBoost:
                if (isNight)
                    growthProgress += growthRate * 0.5f;
                break;

            case SpecialEffectType.ExtraGoldOnHarvest:
                if (Random.value < 0.2f)
                {
                    // PlayerInventory.AddGold(10); // 實作時解除註解
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
                // PlayerInventory.AddItem("AuraFertilizer"); // 實作時解除註解
                break;

            case SpecialEffectType.StableYield:
                quality = Mathf.Clamp(quality, 80f, 100f);
                break;

            case SpecialEffectType.DroughtResistant:
                if ((cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f < 0.3f)
                    growthProgress += growthRate * 0.4f;
                break;
        }
    }

    private void Harvest()
    {
        Debug.Log($"{cropInfo.cropName} 成熟並收成！");
        Destroy(gameObject);
    }
}
