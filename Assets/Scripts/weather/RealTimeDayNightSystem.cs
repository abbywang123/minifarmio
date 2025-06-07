using UnityEngine;
using System;

public class RealTimeDayNightSystem : MonoBehaviour
{
    public static RealTimeDayNightSystem Instance;

    [Header("日夜狀態")]
    public bool IsNight { get; private set; }

    [Header("光照設定（可選）")]
    public Light sunLight;
    public float dayIntensity = 1.0f;
    public float nightIntensity = 0.2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        int hour = DateTime.Now.Hour;
        IsNight = (hour >= 18 || hour < 6);

        if (sunLight != null)
        {
            sunLight.intensity = IsNight ? nightIntensity : dayIntensity;
        }
    }
}
