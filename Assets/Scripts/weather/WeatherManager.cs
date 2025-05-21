using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

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
    public string apiKey = "b98d36786a413e47dc38e0c53ca4e70c"; // 改成你的金鑰
    public string city = "Chiayi";

    [Header("目前天氣資料")]
    public string currentWeather;           // 原始天氣字串
    public WeatherType currentWeatherType;  // 轉為 Enum
    public float temperature;

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
        StartCoroutine(GetWeather());
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

    string FixJson(string value)
    {
        return value;
    }
}

