using UnityEngine;
using UnityEngine.UI;

public class NightOverlayController : MonoBehaviour
{
    public Image nightOverlay;

    void Update()
    {
        if (RealTimeDayNightSystem.Instance == null) return;

        bool isNight = RealTimeDayNightSystem.Instance.IsNight;

        float targetAlpha = isNight ? 0.5f : 0f;
        Color c = nightOverlay.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 2);
        nightOverlay.color = c;
    }
}
