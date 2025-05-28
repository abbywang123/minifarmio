using UnityEngine;

public class FarmTile : MonoBehaviour
{
    public int x, y;
    public string cropId;
    public bool isTilled;
    public int growDays;
    void OnMouseDown()
    {
        FindFirstObjectByType<FarmUIManager>().OpenSeedPopup(this);
    }

    public void Plant(string itemId)
    {
        cropId = itemId;
        growDays = 0;
        isTilled = true;
        Debug.Log($"種植 (itemId) 於 ((x),(y)");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
