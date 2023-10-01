using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public enum ScheduleRegion
{
    None,
    Library,
    Hall
}

public class Manager_Schedules : MonoBehaviour
{
    public void ChangeSchedule(Schedule schedule)
    {

    }
}

[Serializable]
public class Schedule
{
    public List<SchedulePoint> ScheduledRegions;
    public bool IsInScheduledRegion = false;
    public bool IsScheduledTransferring = false;
}

[Serializable]
public class SchedulePoint
{
    public ScheduleRegion ScheduleRegion;
    public BoxCollider2D ScheduleRegionArea;
    public TimesOfDay ScheduleEnter;
    public TimesOfDay ScheduleExit;
    public Vector3 ScheduleTransferTarget;
}
