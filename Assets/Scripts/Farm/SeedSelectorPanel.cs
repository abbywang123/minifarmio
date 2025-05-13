using UnityEngine;
using UnityEngine.UI;

public class SeedSelectorPanel : MonoBehaviour
{
    [SerializeField] private Button seedBtnPrefab;
    [SerializeField] private Transform content;

    private LandTile currentTile;
    private GameObject cropPrefab;

    public void Open(LandTile tile, GameObject prefab)
    {
        currentTile = tile;
        cropPrefab  = prefab;
        gameObject.SetActive(true);
        BuildButtons();
    }
    void BuildButtons()
    {
        foreach (Transform c in content) Destroy(c.gameObject);
        foreach (var crop in Resources.LoadAll<CropData>("Crops"))
        {
            var btn = Instantiate(seedBtnPrefab, content);
            btn.GetComponentInChildren<Text>().text = crop.cropName;
            btn.onClick.AddListener(() => Select(crop));
        }
    }
    void Select(CropData data)
    {
        currentTile.Plant(data, cropPrefab);
        gameObject.SetActive(false);
    }
}
