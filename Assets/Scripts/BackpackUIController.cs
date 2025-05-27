using UnityEngine;

public class BackpackUIController : MonoBehaviour
{
    public GameObject backpackPanel;
    public GameObject backpackOverlay;

    public void OnClickOpenBackpack()
    {
        backpackPanel.SetActive(true);
        backpackOverlay.SetActive(true);
    }

    public void OnClickBackToFarm()
    {
        backpackPanel.SetActive(false);
        backpackOverlay.SetActive(false);
    }
}
