using UnityEngine;

public class CropRuntime : MonoBehaviour
{
    public CropData data;
    [HideInInspector] public int dayCounter;
    [HideInInspector] public int stageIndex;

    private SpriteRenderer sr;

    void OnEnable()  => GameCalendar.OnNewDay += HandleNewDay;
    void OnDisable() => GameCalendar.OnNewDay -= HandleNewDay;

    void HandleNewDay()
    {
        float mult = FindFirstObjectByType<WeatherSystem>().growthMultiplier;

        Grow(mult);
    }

    public void Init(CropData d)
    {
        data = d;
        dayCounter = 0;
        stageIndex = 0;
        sr = GetComponent<SpriteRenderer>();
        UpdateVisual();
    }

    public void Grow(float multiplier)
    {
        dayCounter += Mathf.CeilToInt(multiplier);
        if (dayCounter >= data.stages[stageIndex].days)
        {
            dayCounter = 0;
            stageIndex++;
            if (stageIndex >= data.stages.Count) return; // 已成熟
            UpdateVisual();
        }
    }

    public bool IsMature() => stageIndex >= data.stages.Count;

    void UpdateVisual()
    {
        sr.sprite = data.stages[stageIndex].sprite;
    }
}
