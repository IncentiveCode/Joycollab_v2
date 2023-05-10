using System;
using UnityEngine;

namespace Joycollab.v2
{
    public class AndroidDateCallback : AndroidJavaProxy 
    {
        public static DateTime SelectedDate = DateTime.Now;
        public static bool isDateUpdated;


        public AndroidDateCallback() : base("android.app.DatePickerDialog$OnDateSetListener") { }

        public void onDateSet(AndroidJavaObject view, int year, int month, int day) 
        {
            SelectedDate = new DateTime(year, month + 1, day);
            isDateUpdated = true; 
        }
    }
}