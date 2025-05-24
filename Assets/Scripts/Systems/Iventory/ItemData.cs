using UnityEngine;        // ← 新增這行

[CreateAssetMenu(menuName = "Farm/Item")]
public class ItemData : ScriptableObject
{
    public string id;               // 唯一識別，可用英數
    public string displayName;      // 顯示名稱
    public Sprite icon;             // UI 圖示
    public bool  stackable = true;
    [Range(1, 999)] public int maxStack = 99;
}
