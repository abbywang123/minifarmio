using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeatherUI : MonoBehaviour
{
    [Header("UI 元件")]
    public TextMeshProUGUI weatherText;
    public Image weatherIcon;

    [Header("天氣圖示")]
    public Sprite sunnyIcon;
    public Sprite cloudyIcon;
    public Sprite rainIcon;
    public Sprite defaultIcon;

    public void UpdateWeatherDisplay(string weather, float temperature)
    {
        weatherText.text = $"{weather}  ({temperature}°C)";

        switch (weather)
        {
            case "Clear":
                weatherIcon.sprite = sunnyIcon;
                break;
            case "Clouds":
                weatherIcon.sprite = cloudyIcon;
                break;
            case "Rain":
                weatherIcon.sprite = rainIcon;
                break;
            default:
                weatherIcon.sprite = defaultIcon;
                break;
        }
    }
}
