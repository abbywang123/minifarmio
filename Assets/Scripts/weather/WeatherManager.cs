using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

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
    public string apiKey = "b98d36786a413e47dc38e0c53ca4e70c"; // 你的 OpenWeatherMap API 金鑰
    public string city = "Chiayi";

    [Header("目前天氣資料")]
    public string currentWeather;           // API 傳回的天氣原始字串
    public WeatherType currentWeatherType;  // 對應的 Enum
    public float temperature;               // 溫度（攝氏）

    [Header("UI 元件")]
    public TextMeshProUGUI weatherText;     // 顯示天氣與溫度的 UI 元件

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
            // 請確保 TextMeshPro 使用的是支援全形字元的字型（例如：不要用 LiberationSans）
            weatherText.text = $"天氣: {currentWeatherType}\n溫度: {temperature:0.0}°C";
        }
    }

    string FixJson(string value)
    {
        // 若你需要解析陣列開頭的 JSON，可以在這裡修補格式（這裡不需要）
        return value;
    }
}
