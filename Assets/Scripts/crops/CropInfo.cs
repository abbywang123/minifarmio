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
    [Header("識別 ID")]
    public string seedId;  // 🔑 如：carrotseed

    [Header("基本資料")]
    public string cropName;
    [TextArea(2, 5)]
    public string description;
    public CropType cropType;
    public Sprite icon;

    [Header("成長設定")]
    public float growthRate = 1f;
    [Header("成長階段圖像")]
    public Sprite[] growthStages;

    [Header("環境需求")]
    public float suitableMinTemperature;
    public float suitableMaxTemperature;
    [Range(0f, 1f)]
    public float suitableMinHumidity;
    [Range(0f, 1f)]
    public float suitableMaxHumidity;

    [Header("交配與混種")]
    public bool isHybrid = false;
    public CropInfo parent1;
    public CropInfo parent2;
    public CropInfo[] recommendedBreeds;

    [Header("特殊效果")]
    public SpecialEffectType specialEffect = SpecialEffectType.None;
    [TextArea(2, 5)]
    public string specialEffectDescription;

    [Header("收成物品")]
    public ItemData harvestItem;
}
