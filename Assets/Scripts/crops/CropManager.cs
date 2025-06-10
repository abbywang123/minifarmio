using UnityEngine;
using System;

public class CropManager : MonoBehaviour
{
    public static CropManager Instance { get; private set; }

    public float updateIntervalMinutes = 1f;
    private TimeSpan updateInterval;
    private DateTime lastUpdateTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        updateInterval = TimeSpan.FromMinutes(updateIntervalMinutes);

        LoadLastUpdateTime();
        TryUpdateOnStart();
    }

    private void Update()
    {
        // 用真實時間判斷是否要更新
        DateTime now = DateTime.Now;
        if ((now - lastUpdateTime) >= updateInterval)
        {
            int intervalsPassed = (int)((now - lastUpdateTime).Ticks / updateInterval.Ticks);
            for (int i = 0; i < intervalsPassed; i++)
            {
                AdvanceTimeByInterval();
            }
            lastUpdateTime = lastUpdateTime.AddTicks(updateInterval.Ticks * intervalsPassed);
            SaveLastUpdateTime();
        }
    }

    private void AdvanceTimeByInterval()
    {
        Crop[] crops = FindObjectsOfType<Crop>();
        foreach (var crop in crops)
        {
            crop.UpdateGrowthAuto();
        }
    }

    private void LoadLastUpdateTime()
    {
        string savedTimeStr = PlayerPrefs.GetString("CropManager_LastUpdateTime", string.Empty);
        if (!string.IsNullOrEmpty(savedTimeStr))
        {
            if (DateTime.TryParse(savedTimeStr, out DateTime savedTime))
            {
                lastUpdateTime = savedTime;
                return;
            }
        }

        lastUpdateTime = DateTime.Now;
        SaveLastUpdateTime();
    }

    private void SaveLastUpdateTime()
    {
        PlayerPrefs.SetString("CropManager_LastUpdateTime", lastUpdateTime.ToString());
        PlayerPrefs.Save();
    }

    private void TryUpdateOnStart()
    {
        DateTime now = DateTime.Now;
        if ((now - lastUpdateTime) >= updateInterval)
        {
            int intervalsPassed = (int)((now - lastUpdateTime).Ticks / updateInterval.Ticks);
            for (int i = 0; i < intervalsPassed; i++)
            {
                AdvanceTimeByInterval();
            }
            lastUpdateTime = lastUpdateTime.AddTicks(updateInterval.Ticks * intervalsPassed);
            SaveLastUpdateTime();
        }
    }
}
