using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class CreateAllCropInfos
{
    [MenuItem("Tools/Create All Crops")]
    public static void CreateAllCrops()
    {
        string folder = "Assets/Crops";
        if (!AssetDatabase.IsValidFolder(folder))
            AssetDatabase.CreateFolder("Assets", "Crops");

        Dictionary<string, CropInfo> created = new();

        // 通用作物建立函式
        void Create(string name, string desc, float rate, CropType type, SpecialEffectType effect, string effectDesc, float optTemp, float tolTemp, float optHum, float tolHum)
        {
            var crop = ScriptableObject.CreateInstance<CropInfo>();
            crop.cropName = name;
            crop.description = desc;
            crop.growthRate = rate;
            crop.cropType = type;
            crop.specialEffect = effect;
            crop.specialEffectDescription = effectDesc;
            crop.suitableMinTemperature = optTemp - tolTemp / 2;
            crop.suitableMaxTemperature = optTemp + tolTemp / 2;
            crop.suitableMinHumidity = optHum - tolHum / 2;
            crop.suitableMaxHumidity = optHum + tolHum / 2;
            crop.isHybrid = false;

            AssetDatabase.CreateAsset(crop, $"{folder}/{name}.asset");
            created.Add(name, crop);
        }

        // 普通作物
        Create("草莓", "甜美的草莓", 1.0f, CropType.Normal, SpecialEffectType.None, "無特殊效果", 24, 8, 0.6f, 0.15f);
        Create("玉米", "黃金玉米", 1.2f, CropType.Normal, SpecialEffectType.None, "無特殊效果", 26, 6, 0.65f, 0.15f);
        Create("番茄", "多汁番茄", 1.1f, CropType.Normal, SpecialEffectType.None, "無特殊效果", 24, 8, 0.6f, 0.15f);
        Create("蘿蔔", "根菜類", 1.0f, CropType.Normal, SpecialEffectType.None, "無特殊效果", 20, 7, 0.65f, 0.15f);
        Create("小麥", "常見穀類作物", 1.1f, CropType.Normal, SpecialEffectType.None, "無特殊效果", 22, 6, 0.55f, 0.1f);
        Create("馬鈴薯", "耐寒根莖作物", 1.0f, CropType.Normal, SpecialEffectType.None, "無特殊效果", 18, 5, 0.6f, 0.1f);
        Create("南瓜", "秋季作物", 1.1f, CropType.Normal, SpecialEffectType.None, "無特殊效果", 20, 6, 0.6f, 0.1f);

        // Magic 作物（唯一兩個）
        Create("香菇", "陰暗生長", 0.8f, CropType.Magic, SpecialEffectType.AntiRot, "抗腐爛能力強", 18, 5, 0.8f, 0.1f);
        Create("仙人掌", "沙漠植物", 0.7f, CropType.Magic, SpecialEffectType.DroughtResistant, "耐旱能力強", 30, 10, 0.2f, 0.1f);

        AssetDatabase.SaveAssets();

        // 混種作物
        void CreateHybrid(string name, string desc, float rate, string p1, string p2, SpecialEffectType effect, string effectDesc, float optTemp, float tolTemp, float optHum, float tolHum)
        {
            var crop = ScriptableObject.CreateInstance<CropInfo>();
            crop.cropName = name;
            crop.description = desc;
            crop.growthRate = rate;
            crop.cropType = CropType.Hybrid;
            crop.isHybrid = true;
            crop.parent1 = created[p1];
            crop.parent2 = created[p2];
            crop.specialEffect = effect;
            crop.specialEffectDescription = effectDesc;
            crop.suitableMinTemperature = optTemp - tolTemp / 2;
            crop.suitableMaxTemperature = optTemp + tolTemp / 2;
            crop.suitableMinHumidity = optHum - tolHum / 2;
            crop.suitableMaxHumidity = optHum + tolHum / 2;

            AssetDatabase.CreateAsset(crop, $"{folder}/{name}.asset");
        }

        // 原有混種作物
        CreateHybrid("星星草莓", "夜間成長，鄰近作物品質+5%", 1.1f, "草莓", "香菇", SpecialEffectType.NightBoost, "夜間成長，鄰近作物品質+5%", 24, 8, 0.6f, 0.15f);
        CreateHybrid("火焰玉米", "有機率額外獲得金幣", 1.3f, "玉米", "番茄", SpecialEffectType.ExtraGoldOnHarvest, "有機率額外獲得金幣", 26, 6, 0.65f, 0.15f);
        CreateHybrid("魂之蘿蔔", "可產出靈氣肥料", 1.1f, "蘿蔔", "香菇", SpecialEffectType.ProduceAuraFertilizer, "可產出靈氣肥料", 20, 7, 0.65f, 0.15f);
        CreateHybrid("幻影仙人掌", "外觀幻變，產值穩定", 0.9f, "仙人掌", "草莓", SpecialEffectType.StableYield, "外觀幻變，產值穩定", 30, 10, 0.2f, 0.1f);
        CreateHybrid("泡泡蘑菇", "釋放泡泡的蘑菇", 0.9f, "馬鈴薯", "蘿蔔", SpecialEffectType.ProduceAuraFertilizer, "可產出靈氣肥料", 18, 5, 0.75f, 0.1f);
        CreateHybrid("笑笑蘿蔔", "會笑的蘿蔔", 1.1f, "蘿蔔", "南瓜", SpecialEffectType.ExtraGoldOnHarvest, "有機率獲得額外金幣", 26, 6, 0.65f, 0.1f);

        AssetDatabase.SaveAssets();
        Debug.Log("✅ 所有作物建立完畢！");
    }
}
