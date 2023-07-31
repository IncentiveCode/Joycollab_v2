/// <summary>
/// [mobile] 
/// Mobile 에서 Android 특화 기능을 사용하기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 07. 25
/// @version        : 0.4
/// @update
/// 	v0.1 (2023. 06. 13) : 이전 mobile 에서 작업했던 내용 분리 적용.
///     v0.2 (2023. 07. 06) : DatePicker, TimePicker 에 현재 값을 집어 넣는 테스트 
///     v0.3 (2023. 07. 18) : Joycollab 의 MobilePermission 적용.
///     v0.4 (2023. 07. 25) : TimePicker 현재 값 설정이 이상해서 추가 작업.
/// </summary>

using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

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

        // bridge class, object
        private AndroidJavaClass unityClass;
        private AndroidJavaObject unityActivity;
        private AndroidJavaObject customObject;
        private AndroidJavaClass versionInfo;

    #endif  // for android plugin

        private int sdk_int;


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
        #endif

    #endregion  // Unity functions
        

    #region android bridge functions

        private void SendActivityReference(string packageName)
        {
            Debug.Log(SystemInfo.operatingSystem);

        #if UNITY_ANDROID && !UNITY_EDITOR
        
            unityClass = new AndroidJavaClass(UnityDefaultJavaClassName);
            unityActivity = unityClass.GetStatic<AndroidJavaObject>(UnityCurrentActivity);
            
            // TODO. 작업 끝나면 아래 주석 해제할 것.
            // customObject = new AndroidJavaObject(packageName);
            // customObject.Call(CustomClassReceiveActivityInstanceMethod, unityActivity);

            versionInfo = new AndroidJavaClass("android.os.Build$VERSION");
            sdk_int = versionInfo.GetStatic<int>("SDK_INT");
            Debug.Log("current sdk_int : "+ sdk_int);

        #else

            sdk_int = 33;

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
            double lat = (double) R.singleton.myLat;
            double lon = (double) R.singleton.myLon;

            customObject.Call(CustomClassSetInfo, token, memberSeq, lat, lon);

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

        public void ShowDatepicker(int viewID, string date="") 
        {
            AndroidDateCallback.viewID = viewID;

        #if UNITY_ANDROID && !UNITY_EDITOR
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => PickDate(date)));
        #endif
        }

        private void PickDate(string date="") 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR
            new AndroidJavaObject("android.app.DatePickerDialog", 
                unityActivity, 
                new AndroidDateCallback(date), 
                AndroidDateCallback.SelectedDate.Year, 
                AndroidDateCallback.SelectedDate.Month - 1, 
                AndroidDateCallback.SelectedDate.Day
            ).Call("show");
        #endif
        
        }

        public void ShowTimePicker(int viewID, string time="") 
        {
            AndroidTimeCallback.viewID = viewID;
            var arr = time.Split(':');
            int hour = -1, minute = -1;

            if (arr.Length > 1)
            {
                int.TryParse(arr[0], out hour);
                int.TryParse(arr[1], out minute);
            }

        #if UNITY_ANDROID && !UNITY_EDITOR
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() => PickTime(hour, minute)));
        #endif
        }

        private void PickTime(int h=-1, int m=-1) 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR
            new AndroidJavaObject("android.app.TimePickerDialog", 
                unityActivity, 
                new AndroidTimeCallback(h, m), 
                AndroidTimeCallback.SelectedHour, 
                AndroidTimeCallback.SelectedMinute, 
                true
            ).Call("show");
        #endif
        }

    #endregion  // android bridge functions


    #region android permission check

        public async UniTaskVoid CheckMeetingPermission(System.Action func) 
        {
            await UniTask.DelayFrame(1);

            if (! Permission.HasUserAuthorizedPermission(Permission.Camera)) 
            {
                Permission.RequestUserPermission(Permission.Camera);
                await UniTask.Delay(100);
                await UniTask.WaitUntil(() => Application.isFocused == true);

                if (! Permission.HasUserAuthorizedPermission(Permission.Camera)) 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenConfirm(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "카메라 권한 요청", currentLocale),
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "허용", currentLocale),
                        () => Debug.Log($"{TAG} | 카메라 권한 요청 승인"),
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "거부", currentLocale),
                        () => {
                            PopupBuilder.singleton.OpenAlert(
                                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "카메라 권한 요청 거부", currentLocale)
                            );
                        }
                    );
                    return;
                }
            }
            else 
            {
                Debug.Log($"{TAG} | 이미 승인된 권한 : 카메라 사용");
            }

            await UniTask.DelayFrame(1);

            if (! Permission.HasUserAuthorizedPermission(Permission.Microphone)) 
            {
                Permission.RequestUserPermission(Permission.Microphone);
                await UniTask.Delay(100);
                await UniTask.WaitUntil(() => Application.isFocused == true);

                if (! Permission.HasUserAuthorizedPermission(Permission.Microphone)) 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenConfirm(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "마이크 권한 요청", currentLocale),
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "허용", currentLocale),
                        () => Debug.Log($"{TAG} | 마이크 권한 요청 승인"),
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "거부", currentLocale),
                        () => {
                            PopupBuilder.singleton.OpenAlert(
                                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "마이크 권한 요청 거부", currentLocale)
                            );
                        }
                    );
                    return;
                }
            }
            else 
            {
                Debug.Log($"{TAG} | 이미 승인된 권한 : 마이크 사용");
            }

            func?.Invoke();
        }

        public async UniTaskVoid CheckVoicePermission(System.Action func) 
        {
            await UniTask.DelayFrame(1);

            if (! Permission.HasUserAuthorizedPermission(Permission.Microphone)) 
            {
                Permission.RequestUserPermission(Permission.Microphone);
                await UniTask.Delay(100);
                await UniTask.WaitUntil(() => Application.isFocused == true);

                if (! Permission.HasUserAuthorizedPermission(Permission.Microphone)) 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenConfirm(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "마이크 권한 요청", currentLocale),
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "허용", currentLocale),
                        () => {
                            Debug.Log($"{TAG} | 마이크 권한 요청 승인");
                            func?.Invoke();
                        },
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "거부", currentLocale),
                        () => {
                            PopupBuilder.singleton.OpenAlert(
                                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "마이크 권한 요청 거부", currentLocale)
                            );
                        }
                    );
                    return;
                }
            }
            else 
            {
                Debug.Log($"{TAG} | 이미 승인된 권한 : 마이크 사용");
                func?.Invoke();
            }
        }

        public bool CheckReadStorageAsync() 
        {
            if (sdk_int >= 33) 
            {
                string[] permissions = {
                    "android.permission.READ_MEDIA_AUDIO", 
                    "android.permission.READ_MEDIA_IMAGES",
                    "android.permission.READ_MEDIA_VIDEO"
                };

                bool hasAll = true;
                foreach (string p in permissions) 
                {
                    if (! Permission.HasUserAuthorizedPermission(p)) 
                    {
                        hasAll = false;
                        break;
                    }
                }

                if (! hasAll) 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenConfirm(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "파일읽기 권한 요청", currentLocale),
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "허용", currentLocale),
                        () => {
                            Debug.Log($"{TAG} | 파일 읽기 권한 요청 승인");
                            Permission.RequestUserPermissions(permissions);
                        },
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "거부", currentLocale),
                        () => {
                            PopupBuilder.singleton.OpenAlert(
                                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "파일읽기 권한 요청 거부", currentLocale)
                            );
                        }
                    );

                    return false;
                }
                else 
                {
                    Debug.Log($"{TAG} | sdk version ( >= 33 ), 파일 읽기 권한 모두를 허용함.");
                    return true;
                }
            }
            else 
            {
                if (! Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenConfirm(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "파일읽기 권한 요청", currentLocale),
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "허용", currentLocale),
                        () => {
                            Debug.Log($"{TAG} | 파일 읽기 권한 요청 승인");
                            Permission.RequestUserPermission(Permission.ExternalStorageRead);
                        },
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "거부", currentLocale),
                        () => {
                            PopupBuilder.singleton.OpenAlert(
                                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "파일읽기 권한 요청 거부", currentLocale)
                            );
                        }
                    );

                    return false;
                }
                else 
                {
                    Debug.Log($"{TAG} | sdk version ( < 33 ), 파일 읽기 권한 모두를 허용함.");
                    return true;
                }
            }
        }

        public async UniTaskVoid CheckReadStorageAsync(System.Action func) 
        {
            await UniTask.DelayFrame(1);

            if (! Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
                await UniTask.Delay(100);
                await UniTask.WaitUntil(() => Application.isFocused == true);

                if (! Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenConfirm(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "파일읽기 권한 요청", currentLocale),
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "허용", currentLocale),
                        () => {
                            Debug.Log($"{TAG} | 파일 읽기 권한 요청 승인");
                            func?.Invoke();
                        },
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "거부", currentLocale),
                        () => {
                            PopupBuilder.singleton.OpenAlert(
                                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "파일읽기 권한 요청 거부", currentLocale)
                            );
                        }
                    );
                    return;
                }                
            }
            else 
            {
                func?.Invoke();
            }
        }

        public async UniTaskVoid CheckWriteStorageAsync(System.Action func) 
        {
            await UniTask.DelayFrame(1);

            if (! Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                await UniTask.Delay(100);
                await UniTask.WaitUntil(() => Application.isFocused == true);

                if (! Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    PopupBuilder.singleton.OpenConfirm(
                        LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "파일쓰기 권한 요청", currentLocale),
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "허용", currentLocale),
                        () => {
                            Debug.Log($"{TAG} | 파일 쓰기 권한 요청 승인");
                            func?.Invoke();
                        },
                        LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "거부", currentLocale),
                        () => {
                            PopupBuilder.singleton.OpenAlert(
                                LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "파일쓰기 권한 요청 거부", currentLocale)
                            );
                        }
                    );
                    return;
                }                
            }
            else 
            {
                func?.Invoke();
            }
        }

    #endregion  // android permission check
    }
}