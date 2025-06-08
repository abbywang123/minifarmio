using UnityEngine;

public class FarmUIManager : MonoBehaviour
{
    public SelectSeedPopup popup;

    LandTile currentTile; // ✅ 用 LandTile 取代 FarmTile

    public void OpenSeedPopup(LandTile tile)
    {
        currentTile = tile;
        popup.Show(tile);
    }

    public void PlantSelected(string itemId)
    {
        currentTile.Plant(CropDatabase.GetCropBySeedId(itemId), Resources.Load<GameObject>("Prefabs/SeedlingPrefab"));
        popup.gameObject.SetActive(false);
    }
}
