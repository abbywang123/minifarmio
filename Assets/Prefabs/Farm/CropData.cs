using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Farm/Crop Data")]
public class CropData : ScriptableObject
{
    [Header("靜態資料")]
    public string cropName;
    public Sprite icon;
    public int baseDays;              // 總天數
    public int harvestYield = 1;      // 收成數量

    [Header("各階段設定")]
    public List<StageInfo> stages = new();

    [System.Serializable]
    public class StageInfo
    {
        public string name;
        public int days;              // 該階段所需天數
        public Sprite sprite;         // 階段圖片
    }

    [Header("採收與種子設定")]
    public ItemData seedItem;        // 播種時消耗的種子
    public ItemData harvestItem;     // 成熟後掉落的背包物品
}
