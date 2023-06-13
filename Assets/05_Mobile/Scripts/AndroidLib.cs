/// <summary>
/// [mobile] 
/// Mobile 에서 Android 특화 기능을 사용하기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 06. 13
/// @version        : 0.1
/// @update
/// 	v0.1 (2023. 06. 13) : 이전 mobile 에서 작업했던 내용 분리 적용.
/// </summary>

using UnityEngine;
using TMPro;

namespace Joycollab.v2 
{
    public class AndroidLib : MonoBehaviour
    {
        public static AndroidLib singleton { get; private set; } 
        private const string TAG = "AndroidLib";

    #if UNITY_ANDROID // for android plugin

        // const strings
        //  - common
        private const string PackageName = "kr.co.pitchsolution.joycollab.bridge.Bridge";
        private const string UnityDefaultJavaClassName = "com.unity3d.player.UnityPlayer";
        private const string UnityCurrentActivity = "currentActivity";

        //  - bridge method list
        private const string CustomClassReceiveActivityInstanceMethod = "receiveActivityInstance";
        private const string CustomClassLogMethod = "debugLog";
        private const string CustomClassToastMethod = "showToast";
        private const string CustomClassSetInfo = "setInfo";
        private const string CustomClassStartServiceMethod = "startService";
        private const string CustomClassStopServiceMethod = "stopService";


        private AndroidJavaClass unityClass;
        private AndroidJavaObject unityActivity;
        private AndroidJavaObject customObject;
        private AndroidJavaClass versionInfo;
        private int sdk_int;
        private TMP_Text txtTarget;
    
    #endif  // for android plugin


    #region Unity functions

        #if UNITY_ANDROID
        private void Awake() 
        {
            singleton = this;
        }

        private void Start() 
        {
            SendActivityReference(PackageName); 
        }

        private void Update() 
        {
            // date picker
            if (AndroidDateCallback.isDateUpdated) 
            {
                if (txtTarget != null) 
                {
                    txtTarget.text = AndroidDateCallback.SelectedDate.ToString("yyyy-MM-dd");
                }

                AndroidDateCallback.isDateUpdated = false;
            }

            // time picker
            if (AndroidTimeCallback.isTimeUpdated) 
            {
                if (txtTarget != null) 
                {
                    txtTarget.text = string.Format("{0:00}:{1:00}", 
                        AndroidTimeCallback.SelectedHour,
                        AndroidTimeCallback.SelectedMinute
                    );
                }

                AndroidTimeCallback.isTimeUpdated = false;
            }
        }
        #endif

    #endregion  // Unity functions
        

    #region android bridge functions

        private void SendActivityReference(string packageName)
        {
            Debug.Log(SystemInfo.operatingSystem);

        #if UNITY_ANDROID && !UNITY_EDITOR
        
            unityClass = new AndroidJavaClass(UnityDefaultJavaClassName);
            unityActivity = unityClass.GetStatic<AndroidJavaObject>(UnityCurrentActivity);
            
            customObject = new AndroidJavaObject(packageName);
            customObject.Call(CustomClassReceiveActivityInstanceMethod, unityActivity);

            versionInfo = new AndroidJavaClass("android.os.Build$VERSION");
            sdk_int = versionInfo.GetStatic<int>("SDK_INT");
            Debug.Log("current sdk_int : "+ sdk_int);

        #endif
        }        

        public void DebugLog(string msg) 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR

            customObject.Call(CustomClassLogMethod, msg);

        #endif
        }

        public void ShowToast(string msg) 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR

            customObject.Call(CustomClassToastMethod, msg);
        
        #endif
        }  

        public void SetInfo() 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR

            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;
            // double lat = (double) R.singleton.Lat;
            // double lon = (double) R.singleton.Lng;

            // customObject.Call(CustomClassSetInfo, token, memberSeq, lat, lon);
            customObject.Call(CustomClassSetInfo, token, memberSeq, 0d, 0d);

        #endif
        }

        public void StartService() 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR

            /**
            if (sdk_int >= 29) 
            {
                StartCoroutine(MobilePermission.Instance.LocationPermissionCheck(() => {
                    customObject.Call(CustomClassStartServiceMethod);
                })); 
            }
            else 
            {
                StartCoroutine(MobilePermission.Instance.LocationPermissionCheckWithoutBackground(() => {
                    customObject.Call(CustomClassStartServiceMethod);
                })); 
            }
             */
        
        #endif
        }

        public void StopService()
        {
        #if UNITY_ANDROID && !UNITY_EDITOR

            /**
            if (sdk_int >= 29)
            {
                StartCoroutine(MobilePermission.Instance.LocationPermissionCheck(() => {

                    // StartCoroutine(MobilePermission.Instance.WritePermissionCheck(() => {
                        customObject.Call(CustomClassStopServiceMethod);
                    // }));

                }));
            }
            else 
            {
                StartCoroutine(MobilePermission.Instance.LocationPermissionCheckWithoutBackground(() => {

                    // StartCoroutine(MobilePermission.Instance.WritePermissionCheck(() => {
                        customObject.Call(CustomClassStopServiceMethod);
                    // }));

                })); 
            }
             */

        #endif
        }

        public void ShowDatepicker(TMP_Text target, string title) 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR
            txtTarget = target;

            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => PickDate()));
        #endif
        }

        private void PickDate() 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR
            new AndroidJavaObject("android.app.DatePickerDialog", 
                unityActivity, 
                new AndroidDateCallback(), 
                AndroidDateCallback.SelectedDate.Year, 
                AndroidDateCallback.SelectedDate.Month - 1, 
                AndroidDateCallback.SelectedDate.Day
            ).Call("show");
        #endif
        }

        public void ShowTimePicker(TMP_Text target, string title) 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR
            txtTarget = target;

            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => PickTime()));
        #endif
        }

        private void PickTime() 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR
            int hour = System.DateTime.Now.Hour;
            int minute = System.DateTime.Now.Minute;
            new AndroidJavaObject("android.app.TimePickerDialog", 
                unityActivity, 
                new AndroidTimeCallback(), 
                hour, 
                minute, 
                true
            ).Call("show");
        #endif
        }

    #endregion  // android bridge functions
    }
}