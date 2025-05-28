using UnityEngine;

public class FarmUIManager : MonoBehaviour
{
    public SelectSeedPopup popup;

    FarmTile currentTile;

    public void OpenSeedPopup(FarmTile tile)
    {
        currentTile = tile;
        popup.Show(tile);
    }

    public void PlantSelected(string itemId)
    {
        currentTile.Plant(itemId);
        popup.gameObject.SetActive(false);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
