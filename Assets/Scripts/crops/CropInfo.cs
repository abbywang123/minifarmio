using UnityEngine;

public enum CropType
{
    Normal,
    Magic,
    Hybrid
}

public enum SpecialEffectType
{
    None,
    NightBoost,
    ExtraGoldOnHarvest,
    RainGrowthBoost,
    AntiRot,
    ProduceAuraFertilizer,
    StableYield,
    DroughtResistant
}

[CreateAssetMenu(fileName = "New Crop", menuName = "Crop/Create New Crop")]
public class CropInfo : ScriptableObject
{
    [Header("🔑 對應種子 ID (itemId)")]
    public string seedId; // 需與 ItemDatabase 中的種子 ID 一致，如 "carrotseed"

    [Header("📦 基本資料")]
    public string cropName;
    [TextArea(2, 5)]
    public string description;
    public CropType cropType;
    public Sprite icon;

    [Header("🌱 成長設定")]
    public float growthRate = 1f;

    [Tooltip("依序代表不同成長階段的圖像（例如：幼苗、成熟）")]
    public Sprite[] growthStages;

    [Header("🌡️ 環境需求")]
    public float suitableMinTemperature;
    public float suitableMaxTemperature;
    [Range(0f, 1f)]
    public float suitableMinHumidity;
    [Range(0f, 1f)]
    public float suitableMaxHumidity;

    [Header("🧬 混種與推薦育種")]
    public bool isHybrid = false;
    public CropInfo parent1;
    public CropInfo parent2;
    public CropInfo[] recommendedBreeds;

    [Header("✨ 特殊效果")]
    public SpecialEffectType specialEffect = SpecialEffectType.None;
    [TextArea(2, 5)]
    public string specialEffectDescription;

    [Header("🎁 收成產出 (背包物品)")]
    public ItemData harvestItem;
}
