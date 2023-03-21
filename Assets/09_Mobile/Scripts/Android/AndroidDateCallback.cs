using System;
using UnityEngine;

public class AndroidDateCallback : AndroidJavaProxy 
{
#region Static Variables
    public static DateTime SelectedDate = DateTime.Now;
    public static bool isDateUpdated;
#endregion


    public AndroidDateCallback() : base("android.app.DatePickerDialog$OnDateSetListener") { }

    public void onDateSet(AndroidJavaObject view, int year, int month, int day) 
    {
        SelectedDate = new DateTime(year, month + 1, day);
        isDateUpdated = true; 
    }
}
