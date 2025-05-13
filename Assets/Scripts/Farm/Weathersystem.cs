using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    public enum Weather { Sunny, Cloudy, Rainy }
    [HideInInspector] public Weather today;
    public float growthMultiplier;

    void Start() => RollWeather();

    public void RollWeather()
    {
        today = (Weather)Random.Range(0, 3);
        growthMultiplier = today switch
        {
            Weather.Sunny  => 1.0f,
            Weather.Cloudy => 0.9f,
            Weather.Rainy  => 0.8f,
            _ => 1.0f
        };
        Debug.Log($"[Weather] {today}, mult={growthMultiplier}");
    }
}
