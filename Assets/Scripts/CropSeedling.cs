using UnityEngine;

public class CropSeedling : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public void SetCrop(string itemId)
    {
        // 自動抓一次
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Sprite sprite = CropIconDatabase.GetSpriteById(itemId);
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"❌ 找不到對應作物圖示 for {itemId}");
        }
    }
}


