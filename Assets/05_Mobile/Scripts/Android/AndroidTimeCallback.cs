/// <summary>
/// [mobile - android]
/// Android 에서 time picker 사용시 결과물을 정리하기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 06. 29
/// @version        : 0.2
/// @update
///     v0.1 (2023. 05. 10) : 이전 작업물 이관.
///     v0.2 (2023. 06. 30) : 호출한 화면의 ID 를 함께 가지고 있도록 조치.
/// </summary>

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

        public void onTimeSet(AndroidJavaObject view, int hour, int minute) 
        {
            SelectedHour = hour;
            SelectedMinute = minute;
            isTimeUpdated = true;
        }
    }
}