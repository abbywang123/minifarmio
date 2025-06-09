using UnityEngine;

public static class DragItemData
{
    /// <summary>
    /// 當前拖曳中的物品 ID（對應 ItemData.id）
    /// </summary>
    public static string draggingItemId = null;

    /// <summary>
    /// 拖曳中的 Sprite，用來顯示圖示（選擇性擴充）
    /// </summary>
    public static Sprite draggingIcon = null;

    /// <summary>
    /// 清除拖曳狀態
    /// </summary>
    public static void Clear()
    {
        draggingItemId = null;
        draggingIcon = null;
    }
}

