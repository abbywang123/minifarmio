using UnityEngine;

public class LandTile : MonoBehaviour
{
    public bool isTilled;
    public CropRuntime plantedCrop;

    [SerializeField] private Transform cropRoot; // Sprites 擺放位置
    [SerializeField] private SpriteRenderer soilSr;
    [SerializeField] private Sprite soilNormal;
    [SerializeField] private Sprite soilTilled;

    public bool CanPlant() => isTilled && plantedCrop == null;
    public bool HasCrop()  => plantedCrop != null;

    public void Till()
    {
        isTilled = true;
        soilSr.sprite = soilTilled;
    }

    public void Plant(CropData data, GameObject cropPrefab)
    {
        var go = Instantiate(cropPrefab, cropRoot.position, Quaternion.identity, cropRoot);
        plantedCrop = go.GetComponent<CropRuntime>();
        plantedCrop.Init(data);
    }

    public void Harvest()
    {
        Destroy(plantedCrop.gameObject);
        plantedCrop = null;
        isTilled = false;
        soilSr.sprite = soilNormal;
    }
}
