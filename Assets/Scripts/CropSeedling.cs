using UnityEngine;
using System.Collections;

public class CropSeedling : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    private string currentSeedId; // ✅ 需要記錄播種的種子 ID

    private LandTile parentTile;


    public void SetCrop(string itemId, LandTile tile)
    {
        parentTile = tile;
        currentSeedId = itemId; // ✅ 記住給哪個種子

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

        // ✅ 啟動轉換成 Crop 的協程
        StartCoroutine(GrowIntoCrop());
    }

    private IEnumerator GrowIntoCrop()
    {
        yield return new WaitForSeconds(1f); // 播種動畫過場

        CropInfo info = CropDatabase.GetCropBySeedId(currentSeedId);
        if (info != null)
        {
            var crop = gameObject.AddComponent<Crop>();
            crop.Init(info, parentTile);

            parentTile.SetPlantedCrop(crop); // ✅ 在這裡呼叫！

            Destroy(this); // ✅ 移除 Seedling 階段腳本
        }
        else
        {
            Debug.LogError($"❌ 找不到 CropInfo for 種子ID：{currentSeedId}");
        }
    }


}
