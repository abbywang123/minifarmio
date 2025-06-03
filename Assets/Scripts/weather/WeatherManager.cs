using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class WeatherData
{
    public Weather[] weather;
    public Main main;
}

[System.Serializable]
public class Weather
{
    public string main;
    public string description;
}

[System.Serializable]
public class Main
{
    public float temp;
}

public enum WeatherType
{
    Sunny,
    Rain,
    Cloudy,
    Snow,
    Unknown
}

public class WeatherManager : MonoBehaviour
{
    public static WeatherManager Instance;

    [Header("API 設定")]
    public string apiKey = "b98d36786a413e47dc38e0c53ca4e70c";
    public string city = "Chiayi";

    [Header("目前天氣資料")]
    public string currentWeather;
    public WeatherType currentWeatherType;
    public float temperature;

    [Header("UI 元件")]
    public TextMeshProUGUI weatherText;

    [Header("視覺效果元件")]
    public ParticleSystem rainSystem;
    public Light sunLight;

    [Header("後處理設定（可選）")]
    public Volume postProcessingVolume;
    private ColorAdjustments colorAdjustments;

    void Awake()
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

    void Start()
    {
        StartCoroutine(GetWeatherRoutine());

        if (postProcessingVolume != null)
        {
            postProcessingVolume.profile.TryGet(out colorAdjustments);
        }
    }

    IEnumerator GetWeatherRoutine()
    {
        while (true)
        {
            yield return StartCoroutine(GetWeather());
            yield return new WaitForSeconds(600f); // 每 10 分鐘更新一次
        }
    }

    IEnumerator GetWeather()
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                WeatherData data = JsonUtility.FromJson<WeatherData>(FixJson(json));

                currentWeather = data.weather[0].main;
                temperature = data.main.temp;
                currentWeatherType = ConvertToEnum(currentWeather);

                UpdateWeatherUI();
                UpdateWeatherVisuals();

                Debug.Log($"🌦️ 天氣：{currentWeatherType}，溫度：{temperature}°C");
            }
            else
            {
                Debug.LogError("❌ 取得天氣資料失敗：" + request.error);
            }
        }
    }

    WeatherType ConvertToEnum(string weather)
    {
        switch (weather)
        {
            case "Clear": return WeatherType.Sunny;
            case "Rain":
            case "Drizzle": return WeatherType.Rain;
            case "Clouds": return WeatherType.Cloudy;
            case "Snow": return WeatherType.Snow;
            default: return WeatherType.Unknown;
        }
    }

    void UpdateWeatherUI()
    {
        if (weatherText != null)
        {
            weatherText.text = $"天氣: {currentWeatherType}\n溫度: {temperature:0.0}°C";
        }
    }

    string FixJson(string value)
    {
        return value;
    }

    void UpdateWeatherVisuals()
    {
        switch (currentWeatherType)
        {
            case WeatherType.Sunny:
                if (rainSystem.isPlaying) rainSystem.Stop();
                sunLight.intensity = 1.2f;
                sunLight.color = Color.white;
                RenderSettings.ambientLight = Color.white;
                break;

            case WeatherType.Cloudy:
                if (rainSystem.isPlaying) rainSystem.Stop();
                sunLight.intensity = 0.6f;
                sunLight.color = new Color(0.8f, 0.8f, 0.9f);
                RenderSettings.ambientLight = new Color(0.6f, 0.6f, 0.7f);
                break;

            case WeatherType.Rain:
                if (!rainSystem.isPlaying) rainSystem.Play();
                sunLight.intensity = 0.3f;
                sunLight.color = new Color(0.6f, 0.6f, 0.7f);
                RenderSettings.ambientLight = new Color(0.4f, 0.4f, 0.5f);
                break;

            case WeatherType.Snow:
                if (!rainSystem.isPlaying) rainSystem.Play(); // 可換為雪特效
                sunLight.intensity = 0.7f;
                sunLight.color = new Color(0.9f, 0.9f, 1.0f);
                RenderSettings.ambientLight = new Color(0.8f, 0.8f, 1.0f);
                break;

            default:
                if (rainSystem.isPlaying) rainSystem.Stop();
                sunLight.intensity = 0.5f;
                break;
        }

        // 夜晚降亮度（依系統時間）
        if (System.DateTime.Now.Hour >= 18 || System.DateTime.Now.Hour < 6)
        {
            sunLight.intensity *= 0.3f;
            RenderSettings.ambientLight *= 0.3f;

            if (colorAdjustments != null)
                colorAdjustments.postExposure.value = -0.5f;
        }
        else
        {
            if (colorAdjustments != null)
                colorAdjustments.postExposure.value = 0f;
        }
    }
}
