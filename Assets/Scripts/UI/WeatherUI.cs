using UnityEngine;
using TMPro;

public class WeatherUI : MonoBehaviour
{
    public TextMeshProUGUI weatherText;

    public void SetWeather(string weatherType, float temperature)
    {
        weatherText.text = $"天氣：{ConvertWeatherToChinese(weatherType)}　溫度：{Mathf.RoundToInt(temperature)}°C";
    }

    private string ConvertWeatherToChinese(string weatherType)
    {
        switch (weatherType)
        {
            case "Sunny": return "晴天";
            case "Rainy": return "雨天";
            case "Cloudy": return "多雲";
            case "Snowy": return "下雪";
            default: return weatherType;
        }
    }
}
