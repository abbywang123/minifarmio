using UnityEngine;

public class CropManager : MonoBehaviour
{
    public void AdvanceDay()
    {
        Crop[] crops = FindObjectsOfType<Crop>();
        foreach (var crop in crops)
        {
            crop.UpdateGrowthAuto();
        }
    }
}
