using UnityEngine;

public class CropRuntime : MonoBehaviour
{
    public CropData data;
    [HideInInspector] public int dayCounter;
    [HideInInspector] public int stageIndex;

    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

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
        UpdateVisual();
    }

    public void Grow(float multiplier)
    {
        if (data == null || stageIndex >= data.stages.Count)
        {
            Debug.LogWarning($"[CropRuntime] 無法執行 Grow：資料未初始化或已成熟 ({gameObject.name})");
            return;
        }

        dayCounter += Mathf.CeilToInt(multiplier);

        if (dayCounter >= data.stages[stageIndex].days)
        {
            dayCounter = 0;
            stageIndex++;

            if (stageIndex >= data.stages.Count)
                return; // 成熟，停止成長

            UpdateVisual();
        }
    }

    public bool IsMature() => stageIndex >= data.stages.Count;

    void UpdateVisual()
    {
        if (sr == null)
        {
            Debug.LogWarning($"[CropRuntime] SpriteRenderer 為 null，自動嘗試取得 ({gameObject.name})");
            sr = GetComponent<SpriteRenderer>();
        }

        if (data == null || stageIndex >= data.stages.Count)
        {
            Debug.LogWarning($"[CropRuntime] 無法更新圖像：資料未初始化或 stageIndex 越界 ({gameObject.name})");
            return;
        }

        sr.sprite = data.stages[stageIndex].sprite;
    }

    public void Harvest()
    {
        var player = FindFirstObjectByType<Player>();

        if (player == null)
        {
            Debug.LogWarning("[CropRuntime] 找不到 Player 物件，無法收成。");
            return;
        }

        var ok = player.GetComponent<Inventory>()
                       .Add(data.harvestItem, 1);

        if (!ok)
        {
            WarehouseManager.Instance.inventory.Add(data.harvestItem, 1);
        }

        Destroy(gameObject);
    }
}
