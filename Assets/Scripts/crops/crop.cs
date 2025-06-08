using UnityEngine;
using System.Collections;

public class Crop : MonoBehaviour
{
    public CropInfo cropInfo;

    private float growthProgress = 0f;
    private float health = 100f;
    private float quality = 100f;

    private SpriteRenderer spriteRenderer;
    private int currentStage = -1;
    private Sprite[] growthStages;

    public float GetGrowthProgress() => growthProgress;
    public float GetHealth() => health;
    public float GetQuality() => quality;

    public CropInfo GetInfo() => cropInfo;
    public float GetGrowthProgressNormalized() => Mathf.Clamp01(growthProgress / 100f);
    public float GetHealthNormalized() => Mathf.Clamp01(health / 100f);
    public float GetQualityNormalized() => Mathf.Clamp01(quality / 100f);



    private float growthRate => cropInfo.growthRate;

    private void Awake()
    {
        // 抓取 SpriteObject 子物件上的 SpriteRenderer
        spriteRenderer = transform.Find("SpriteObject").GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        Debug.LogError("❌ 沒有找到 SpriteRenderer，請檢查 SpriteObject 是否存在且掛載正確！");
    }

    private void Start()
    {
        // 測試：一開始顯示第一階段圖片
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

    public void Init(CropInfo info)
    {
        cropInfo = info;
        growthProgress = 0f;
        health = 100f;
        quality = 100f;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // 加入這段：用 CropIconDatabase 設定初始圖示
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
        growthStages = new Sprite[2];
        for (int i = 0; i < growthStages.Length; i++)
        {
            growthStages[i] = Resources.Load<Sprite>($"CropAnimator/{cropInfo.cropName}_{i}");
        }
    }

    private void UpdateVisual()
    {
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
        float optimalTemp = (cropInfo.suitableMinTemperature + cropInfo.suitableMaxTemperature) / 2f;
        float tempTolerance = (cropInfo.suitableMaxTemperature - cropInfo.suitableMinTemperature) / 2f;

        float optimalHumidity = (cropInfo.suitableMinHumidity + cropInfo.suitableMaxHumidity) / 2f;
        float humidityTolerance = (cropInfo.suitableMaxHumidity - cropInfo.suitableMinHumidity) / 2f;

        float tempFactor = Mathf.Clamp01(1 - Mathf.Abs(temperature - optimalTemp) / tempTolerance);
        float humidityFactor = Mathf.Clamp01(1 - Mathf.Abs(humidity - optimalHumidity) / humidityTolerance);
        float weatherFactor = (weather == "Rain") ? 0.9f : 1f;

        growthProgress += growthRate * tempFactor * humidityFactor * weatherFactor;

        if (weather == "Rain" && Random.value < 0.2f)
            health -= 10f;

        ApplySpecialEffect(weather, isNight);

        if (growthProgress >= 100f)
            Harvest();
        UpdateVisual();
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

    private void Harvest()
    {
        Debug.Log($"{cropInfo.cropName} 成熟並收成！");

        var player = FindFirstObjectByType<Player>();
        if (player == null)
        {
            Debug.LogWarning("[Crop] 找不到 Player，無法收成。");
            return;
        }

        bool ok = player.GetComponent<Inventory>()
                .Add(cropInfo.harvestItem, 1);

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
        growthProgress += growthRate * 0.2f; // 增加額外生長進度
        health = Mathf.Min(health + 5f, 100f); // 回復少量健康
        UpdateVisual();
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
        UpdateVisual();
    }
}
