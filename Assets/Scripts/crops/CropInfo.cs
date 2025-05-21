// CropInfo.cs
using UnityEngine;

public enum SpecialEffectType
{
    None,
    NightBoost,
    ExtraGoldOnHarvest,
    RainGrowthBoost,
    AntiRot,
    ProduceAuraFertilizer,
    StableYield
}

[CreateAssetMenu(fileName = "New Crop", menuName = "Crop/Create New Crop")]
public class CropInfo : ScriptableObject
{
    public string cropName;
    public string description;
    public float growthRate;
    public string cropType; // Normal, Magic, Hybrid
    public Sprite icon;

    public CropInfo[] recommendedBreeds;

    public bool isHybrid;
    public CropInfo parent1;
    public CropInfo parent2;

    public SpecialEffectType specialEffect;
    [TextArea] public string specialEffectDescription;
}