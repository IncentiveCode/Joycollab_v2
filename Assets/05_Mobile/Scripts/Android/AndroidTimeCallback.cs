using System;
using UnityEngine;

namespace Joycollab.v2
{
    public class AndroidTimeCallback : AndroidJavaProxy
    {
        public static int SelectedHour;
        public static int SelectedMinute;
        public static bool isTimeUpdated;


        public AndroidTimeCallback() : base("android.app.TimePickerDialog$OnTimeSetListener") { }

        public void onTimeSet(AndroidJavaObject view, int hour, int minute) 
        {
            SelectedHour = hour;
            SelectedMinute = minute;
            isTimeUpdated = true;
        }
    }
}