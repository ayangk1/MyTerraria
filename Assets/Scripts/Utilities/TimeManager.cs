using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TimeManager : MonoBehaviour
{
    public List<Schedule> schedule;
    private void Start()
    {
        LightHandler.Instance.WorldTimeChanged += CheckSchedule;
    }

    private void CheckSchedule(object sender, TimeSpan newTime)
    {
        var schedule = this.schedule.FirstOrDefault(s => s.hour == (int)(newTime.Hours / 24f * LightHandler.Instance.dayLength) 
                                                         && s.minute == newTime.Minutes);
        
        schedule?.action?.Invoke(schedule.dayTimeState);
    }

    [Serializable]
    public class Schedule
    {
        public int hour;
        public int minute;
        public DayTimeState dayTimeState;
        public UnityEvent<DayTimeState> action;
    }
}
