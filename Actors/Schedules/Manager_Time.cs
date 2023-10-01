using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TimesOfDay
{
    PreDawn, // 03:00 - 06:00
    Dawn, // 06:00 - 09:00
    Morning, // 09:00 - 12:00
    Noon, // 12:00 - 15:00
    Afternoon, // 15:00 - 18:00
    Evening, // 18:00 - 21:00
    Twilight, // 20:00 - 21:00
    Night, // 21:00 - 00:00
    Midnight // 00:00 - 03:00
}

public class Manager_Time : MonoBehaviour
{
    public static Manager_Time Instance;
    public event Action OnNewDay;
    public event Action<TimesOfDay> OnNewTimesOfDay;

    public float WorldTime = 0;
    public float LocalTime = 0;
    public float SecondsPerDay = 864000;
    private float _secondsPerTimePeriod;
    private TimesOfDay _currentTimesOfDay; public TimesOfDay CurrentTimesOfDay {  get { return _currentTimesOfDay; } }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void InitialiseTime()
    {
        _secondsPerTimePeriod = SecondsPerDay / 8.0f;

        _currentTimesOfDay = TimesOfDay.Morning;


        // If we have previously saved a time, start at that time.

        // If not, then start at Morning (09:00).
    }

    public void Update()
    {
        if (LocalTime < SecondsPerDay)
        {
            LocalTime++;
        }
        else
        {
            LocalTime = 0;
            StartNewDay();
        }

        if (LocalTime >= ((int)_currentTimesOfDay + 1) * _secondsPerTimePeriod)
        {
            _currentTimesOfDay = (TimesOfDay)(((int)_currentTimesOfDay + 1) % 8);
            OnNewTimesOfDay?.Invoke(_currentTimesOfDay);
        }
    }

    public void StartNewDay()
    {
        GetWorldTime();
        CheckForWorldEvents();

        OnNewDay?.Invoke();
    }

    public float GetWorldTime()
    {
        // Instead of incrementing the world time every frame, whenever you need it, just add the time elapsed since the previous WorldTime was saved, and add it with the time elapsed.
        // Find out how to make this world cause adding Time.time is not good enough.
        WorldTime += Time.time;

        return WorldTime; 
    }

    public void CheckForWorldEvents()
    {
        //foreach(WorldEvent worldEvent in Manager_WorldEvents.Instance.AllWorldEventsList)
        //{
        //    if (worldEvent.TriggerTime < WorldTime)
        //    {
        //        worldEvent.TriggerEvent();
        //    }
        //}
    }
}
