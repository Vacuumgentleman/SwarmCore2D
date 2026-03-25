using UnityEngine;
using System;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    public enum TimerMode
    {
        CountUp,
        CountDown
    }

    [Header("Mode")]
    public TimerMode mode = TimerMode.CountUp;

    [Header("Time")]
    public float currentTime = 0f;
    public float startTime = 0f; 

    public bool isRunning = true;

    public event Action OnTimeChanged;
    public event Action OnTimerEnd;

    void Awake()
    {
        Instance = this;

        if (mode == TimerMode.CountDown)
            currentTime = startTime;
    }

    void Update()
    {
        if (!isRunning)
            return;

        if (mode == TimerMode.CountUp)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                isRunning = false;
                OnTimerEnd?.Invoke();
            }
        }

        OnTimeChanged?.Invoke();
    }

    public float GetTime()
    {
        return currentTime;
    }

    public string GetFormattedTime()
    {
        int totalSeconds = Mathf.FloorToInt(currentTime);

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        if (hours > 0)
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        else
            return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        if (mode == TimerMode.CountDown)
            currentTime = startTime;
        else
            currentTime = 0f;

        OnTimeChanged?.Invoke();
    }
}