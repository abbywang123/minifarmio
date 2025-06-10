using UnityEngine;

public enum SoilType
{
    Normal,
    Desert,
    Wetland
}

public class LandTile : MonoBehaviour
{
    public int x, y;
    public bool isTilled;
    public Crop plantedCrop;

    [Header("地形種類")]
    [SerializeField] private SoilType soilType = SoilType.Normal;

    [Header("濕度參數")]
    [SerializeField] private float moisture = 0.5f;

    [Header("視覺元件")]
    [SerializeField] private Transform cropRoot;
    [SerializeField] private SpriteRenderer soilSr;
    [SerializeField] private Sprite soilNormal;
    [SerializeField] private Sprite soilTilled;

    private void Start()
    {
        // 根據地形類型設定濕度
        switch (soilType)
        {
            case SoilType.Normal:
                moisture = 0.6f;
                break;
            case SoilType.Desert:
                moisture = 0.2f;
                break;
            case SoilType.Wetland:
                moisture = 0.75f;
                break;
        }
    }

    // ===== 對外方法 =====

    public bool CanPlant() => isTilled && plantedCrop == null;

    public bool HasCrop() => plantedCrop != null;

    public float GetCurrentMoisture() => moisture;

    public void AddMoisture(float amount)
    {
        moisture = Mathf.Clamp01(moisture + amount);
    }

    public void Till()
    {
        isTilled = true;
        soilSr.sprite = soilTilled;
    }

    public void Plant(CropInfo info, GameObject cropPrefab)
    {
        var go = Instantiate(cropPrefab, cropRoot.position, Quaternion.identity, cropRoot);
        plantedCrop = go.GetComponent<Crop>();
        plantedCrop.Init(info, this);
    }

    public void Harvest()
    {
        if (plantedCrop != null)
        {
            Destroy(plantedCrop.gameObject);
            plantedCrop = null;
        }

        isTilled = false;
        soilSr.sprite = soilNormal;
    }

    void OnMouseDown()
    {
        if (plantedCrop != null)
        {
        
            CropInfoPanelManager.Instance.ShowPanel(plantedCrop);

        }
    }

    public void SetPlantedCrop(Crop crop)
    {
        plantedCrop = crop;
    }

}

[System.Serializable]
public class PlantedCropData
{
    public string cropId;
    public float plantedTime;
    public int growthStage;

    public PlantedCropData(string id, float time, int stage)
    {
        cropId = id;
        plantedTime = time;
        growthStage = stage;
    }
}
