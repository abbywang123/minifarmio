using System;
using UnityEngine;

public class GameCalendar : MonoBehaviour
{
    public static event Action OnNewDay;
    public int day;

    [SerializeField] private float realSecondsPerDay = 5f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= realSecondsPerDay)
        {
            timer = 0;
            day++;
            OnNewDay?.Invoke();
        }
    }
}
