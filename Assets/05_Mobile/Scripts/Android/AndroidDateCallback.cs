/// <summary>
/// [mobile - android]
/// Android 에서 Date picker 사용시 결과물을 정리하기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 07. 06
/// @version        : 0.3
/// @update
///     v0.1 (2023. 05. 10) : 이전 작업물 이관.
///     v0.2 (2023. 06. 30) : 호출한 화면의 ID 를 함께 가지고 있도록 조치.
///     v0.3 (2023. 07. 06) : DatePicker 에 현재 값을 넣을 수 있도록 수정.
/// </summary>

using System;
using UnityEngine;

namespace Joycollab.v2
{
    public class AndroidDateCallback : AndroidJavaProxy 
    {
        public static DateTime SelectedDate = DateTime.Now;
        public static bool isDateUpdated;
        public static int viewID;

        public AndroidDateCallback() : base("android.app.DatePickerDialog$OnDateSetListener") { }

        public AndroidDateCallback(string date) : base("android.app.DatePickerDialog$OnDateSetListener") 
        {
            SelectedDate = string.IsNullOrEmpty(date) ? DateTime.Now : Convert.ToDateTime(date);
        }

        public void onDateSet(AndroidJavaObject view, int year, int month, int day) 
        {
            SelectedDate = new DateTime(year, month + 1, day);
            isDateUpdated = true; 
        }
    }
}