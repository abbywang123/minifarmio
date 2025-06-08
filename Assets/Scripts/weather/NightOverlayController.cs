using UnityEngine;
using UnityEngine.UI;

public class NightOverlayController : MonoBehaviour
{
    public Image nightOverlay;

    void Update()
    {
        if (RealTimeDayNightSystem.Instance == null) return;

        bool isNight = RealTimeDayNightSystem.Instance.IsNight;

        // 透明度：夜晚 = 110 / 255，白天 = 0
        float targetAlpha = isNight ? 110f / 255f : 0f;

        Color c = nightOverlay.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 2);

        // 確保不會卡在接近值
        if (Mathf.Abs(c.a - targetAlpha) < 0.01f)
        {
            c.a = targetAlpha;
        }

        nightOverlay.color = c;
    }
}
