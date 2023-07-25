/// <summary>
/// [mobile - android]
/// Android 에서 time picker 사용시 결과물을 정리하기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 07. 06
/// @version        : 0.3
/// @update
///     v0.1 (2023. 05. 10) : 이전 작업물 이관.
///     v0.2 (2023. 06. 30) : 호출한 화면의 ID 를 함께 가지고 있도록 조치.
///     v0.3 (2023. 07. 06) : TimePicker 에 현재 값을 넣을 수 있도록 수정.
/// </summary>

using System;
using UnityEngine;

namespace Joycollab.v2
{
    public class AndroidTimeCallback : AndroidJavaProxy
    {
        public static int SelectedHour;
        public static int SelectedMinute;
        public static bool isTimeUpdated;
        public static int viewID;

        public AndroidTimeCallback() : base("android.app.TimePickerDialog$OnTimeSetListener") { }

        public AndroidTimeCallback(int hour, int minute) : base("android.app.TimePickerDialog$OnTimeSetListener") 
        {
            SelectedHour = hour == -1 ? DateTime.Now.Hour : hour;
            SelectedMinute = minute == -1 ? DateTime.Now.Minute : minute;
        }

        public void onTimeSet(AndroidJavaObject view, int hour, int minute) 
        {
            SelectedHour = hour;
            SelectedMinute = minute;
            isTimeUpdated = true;
        }
    }
}