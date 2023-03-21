using System;
using UnityEngine;

public class AndroidTimeCallback : AndroidJavaProxy
{
#region Static Variables
    public static int SelectedHour;
    public static int SelectedMinute;
    public static bool isTimeUpdated;
#endregion


    public AndroidTimeCallback() : base("android.app.TimePickerDialog$OnTimeSetListener") { }

    public void onTimeSet(AndroidJavaObject view, int hour, int minute) 
    {
        SelectedHour = hour;
        SelectedMinute = minute;
        isTimeUpdated = true;
    }
}
