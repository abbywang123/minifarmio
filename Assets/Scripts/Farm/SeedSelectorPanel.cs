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

        foreach (var crop in Resources.LoadAll<CropInfo>("Crops"))  // ✅ 改成 CropInfo
        {
            var btn = Instantiate(seedBtnPrefab, content);
            btn.GetComponentInChildren<Text>().text = crop.cropName;

            // 用變數包住 crop，避免閉包錯誤
            CropInfo captured = crop;
            btn.onClick.AddListener(() => Select(captured));
        }
    }

    void Select(CropInfo info)
    {
        currentTile.Plant(info, cropPrefab);
        gameObject.SetActive(false);
    }
}
