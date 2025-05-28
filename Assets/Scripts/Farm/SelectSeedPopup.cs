using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SelectSeedPopup : MonoBehaviour
{
    public Transform gridParent;
    public GameObject slotPrefab;
    FarmUIManager manager;

    void Awake() => manager = FindFirstObjectByType<FarmUIManager>();

    public void Show(FarmTile tile)
    {
        gameObject.SetActive(true);
        foreach (Transform c in gridParent) Destroy(c.gameObject);

        foreach (var item in InventoryManager.Instance.GetAllitems())
        {
            var go = Instantiate(slotPrefab, gridParent);
            go.GetComponentInChildren<TMP_Text>().text = item.itemId;
            go.GetComponent<Button>().onClick.AddListener(() =>
            {
                manager.PlantSelected(item.itemId);
            });
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
