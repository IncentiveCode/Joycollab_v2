/// <summary>
/// [mobile]
/// 사무실 화면을 관리하는 클래스. (Gesture 와 GPS 관련 기능을 사용)
/// @author         : HJ Lee
/// @last update    : 2023. 02. 22
/// @version        : 0.3
/// @update
///     v0.1 : 최초 생성 
///     v0.2 (2022. 05. 26) : 이벤트 핸들러 추가.
///     v0.3 (2023. 03. 22) : FixedView 실험, UniTask 적용, UI Canvas 최적화. Gesture Detect 최적화.
/// </summary>

using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.InputSystem.Controls;
#endif

namespace Joycollab.v2
{
    public class OfficeM : FixedView
    {
        [Header("GPS")]
        [SerializeField] private Toggle _toggleGpsOff;
        [SerializeField] private GameObject _goGpsInfo;
        [SerializeField] private Text _txtLat;
        [SerializeField] private Text _txtLon;
        [SerializeField] private Text _txtDistance;

        [Header("Gesture")]
        [SerializeField] private Button _btnGesture;
        [SerializeField] private GameObject _goGestureGuide;
        [SerializeField] private Text _txtLog;
        [SerializeField] private Button _btnReDetect;
        [SerializeField] private Button _btnCloseGuide;
        

        // Local variables
        //  - for gesture detect
        private const float RadToDeg = 57.295779513082320876798154814105f;
        private bool isDetect;
        private float timer;

        //  - for calculate
        private GestureDetectProperties gestureProperties;
        private Vector3 v3Accel, v3AbsAccel;
        private Vector3 v3Gyro, v3AbsGyro;
        private Vector3 v3Attitude, v3AbsAttitude;
        private EulerAngles eaAttitude;
        private float linearForceScalar;

        //  - for result display
        private List<int> counter;
        private List<float> percent;
        private List<int> actGesture;
        private StringBuilder builder;


    #region Unity functions
        private void Awake() 
        {
            Init();
            Reset();
        }

        #if UNITY_ANDROID || UNITY_IOS
        private void Update() 
        {
            // gps info update
            _txtLat.text = string.Format("lat : {0}", GpsManager.Instance.currLat);
            _txtLon.text = string.Format("lon : {0}", GpsManager.Instance.currLon);
            _txtDistance.text = string.Format("distance : {0}", GpsManager.Instance.distance);

            // gesture detection
            if (! isDetect) return;

            timer += Time.deltaTime;
            DetectGesture();
            if (timer > gestureProperties.DetectTime) 
            {
                ExpressGesture();
                HandleGesturePopup(false);
            }
        }
        #endif

        private void OnDestroy() 
        {
            if (MobileEvents.Instance != null) 
            {
                MobileEvents.Instance.OnBackButtonProcess -= BackButtonProcess;
            }
        }
    #endregion  // Unity functions


    #region FixedView functions
        protected override void Init() 
        {
            base.Init();


            // set button listener for GPS
            _toggleGpsOff.onValueChanged.AddListener((off) => {
                if (off) 
                {
                    // GpsManager.Instance.StopGpsModule();
                    _goGpsInfo.SetActive(false);
                }
                else 
                {
                    // GpsManager.Instance.StartGpsModule();
                    _goGpsInfo.SetActive(true);
                }
            });

            // set button listener for GESTURE
            _btnGesture.onClick.AddListener(() => {
                _txtLog.text = "detecting...";
                _btnReDetect.interactable = false;
                _goGestureGuide.SetActive(true);

                HandleGesturePopup(true);
            });
            _btnReDetect.onClick.AddListener(() => {
                _txtLog.text = "detecting...";
                _btnReDetect.interactable = false;

                HandleGesturePopup(true);
            });
            _btnCloseGuide.onClick.AddListener(() => {
                HandleGesturePopup(false);
                _goGestureGuide.SetActive(false);
            });

            // add event handling
            MobileEvents.Instance.OnBackButtonProcess += BackButtonProcess;


            // set local variables
            int[] arr = { 
                (int) eGestureAct.LeftRight, 
                (int) eGestureAct.UpDown, 
                (int) eGestureAct.ScaleDown,
                (int) eGestureAct.ScaleUp,
                (int) eGestureAct.TurnAround,
            };
            actGesture = arr.ToList();
            builder = new StringBuilder();
        }

        protected override void Reset() 
        {
            base.Reset();
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh();

            base.Appearing().Forget();
        }
    #endregion  // FixedView functions


    #region Gesture detection
        private void InitGestureDetector() 
        {
            string prop = JsLib.GetCookie(Key.MOBILE_GESTURE_PROPERTIES);
            gestureProperties = string.IsNullOrEmpty(prop) ?
                new GestureDetectProperties() : 
                JsonUtility.FromJson<GestureDetectProperties>(prop);

            HandleGesturePopup(false);
        }

        private void HandleGesturePopup(bool state) 
        {
            if (state)
            {
                counter = Enumerable.Repeat(0, (int) eGestureMotion.LENGTH).ToList();
                percent = Enumerable.Repeat(0f, (int) eGestureMotion.LENGTH).ToList();
                builder.Clear();
            }
            else 
            {
                counter = null;
                percent = null;
            }

            timer = 0f;
            isDetect = state;

            if (Application.isEditor) return;

            if (state) 
            {
                InputSystem.EnableDevice(UnityEngine.InputSystem.Accelerometer.current);
                InputSystem.EnableDevice(UnityEngine.InputSystem.Gyroscope.current);
                InputSystem.EnableDevice(UnityEngine.InputSystem.AttitudeSensor.current);
            }
            else 
            {
                InputSystem.DisableDevice(UnityEngine.InputSystem.Accelerometer.current);
                InputSystem.DisableDevice(UnityEngine.InputSystem.Gyroscope.current);
                InputSystem.DisableDevice(UnityEngine.InputSystem.AttitudeSensor.current);
            }
        }

        private void DetectGesture() 
        {
            if (Application.isEditor) return;

            // read sensor value and conversion
            v3Accel = UnityEngine.InputSystem.Accelerometer.current.acceleration.ReadValue();
            v3AbsAccel = new Vector3(Mathf.Abs(v3Accel.x), Mathf.Abs(v3Accel.y), Mathf.Abs(v3Accel.z));
            linearForceScalar = v3Accel.magnitude;

            v3Gyro = (UnityEngine.InputSystem.Gyroscope.current.angularVelocity.ReadValue()) * RadToDeg;
            v3AbsGyro = new Vector3(Mathf.Abs(v3Gyro.x), Mathf.Abs(v3Gyro.y), Mathf.Abs(v3Gyro.z));

            eaAttitude = Util.ToEulerAngle(UnityEngine.InputSystem.AttitudeSensor.current.attitude.ReadValue());
            v3Attitude = new Vector3(eaAttitude.Pitch * RadToDeg, eaAttitude.Roll * RadToDeg, eaAttitude.Yaw * RadToDeg);
            v3AbsAttitude = new Vector3(Mathf.Abs(v3Attitude.x), Mathf.Abs(v3Attitude.y), Mathf.Abs(v3Attitude.z));

            // 회전이 없는 경우.
            if (v3AbsGyro.x < gestureProperties.AngularForce && v3AbsGyro.y < gestureProperties.AngularForce && v3AbsGyro.z < gestureProperties.AngularForce)
            {
                // Debug.LogInfo("linearForceScalar : "+ linearForceScalar);
                if (linearForceScalar < gestureProperties.Force) return;

                if ((v3Attitude.y > 40 + gestureProperties.Tolerance) && (v3Attitude.y < 120 - gestureProperties.Tolerance)) 
                {
                    // case N. 앞-뒤로 움직이는 힘이 좌-우/상-하 보다 큰 경우.
                    if(v3AbsAccel.z > v3AbsAccel.x && v3AbsAccel.z > v3AbsAccel.y) 
                    {
                        int i = (int) eGestureMotion.Other;

                        if (percent[i] < (linearForceScalar * 100f) / gestureProperties.Force)
                            percent[i] = (linearForceScalar * 100f) / gestureProperties.Force;

                        counter[i] ++;
                    }
                    // case 0. 좌-우로 움직이는 힘이 앞-뒤/상-하 보다 큰 경우.
                    else if (v3AbsAccel.x > v3AbsAccel.y && v3AbsAccel.x > v3AbsAccel.z) 
                    {
                        int i = (int) eGestureMotion.LeftRight;

                        if (percent[i] < (linearForceScalar * 100f) / gestureProperties.Force)
                            percent[i] = (linearForceScalar * 100f) / gestureProperties.Force;

                        counter[i] ++;
                    }

                    // case 1. 상-하로 움직이는 힘이 좌-우/앞-뒤 보다 큰 경우.
                    else if (v3AbsAccel.y > v3AbsAccel.x && v3AbsAccel.y > v3AbsAccel.z) 
                    {
                        int i = (int) eGestureMotion.UpDown;

                        if (percent[i] < (linearForceScalar * 100f) / gestureProperties.Force)
                            percent[i] = (linearForceScalar * 100f) / gestureProperties.Force;

                        counter[i] ++;
                    }
                }
            }

            // case N. X 축 회전
            else if (v3AbsGyro.x > gestureProperties.AngularForce && v3AbsGyro.x > v3AbsGyro.y && v3AbsGyro.x > v3AbsGyro.y) 
            {
                int i = (int) eGestureMotion.Other;

                if (percent[i] < (v3Gyro.x * 100f) / gestureProperties.AngularForce)
                    percent[i] = (v3Gyro.x * 100f) / gestureProperties.AngularForce;

                counter[i] ++;
            }

            // case N. Y 축 회전
            else if (v3AbsGyro.y > gestureProperties.AngularForce && v3AbsGyro.y > v3AbsGyro.x && v3AbsGyro.y > v3AbsGyro.z) 
            {
                int i = (int) eGestureMotion.Other;

                if (percent[i] < (v3Gyro.y * 100f) / gestureProperties.AngularForce)
                    percent[i] = (v3Gyro.y * 100f) / gestureProperties.AngularForce;

                counter[i] ++;
            }
            
            // Z 축 회전
            else if (v3AbsGyro.z > gestureProperties.AngularForce && v3AbsGyro.z > v3AbsGyro.x && v3AbsGyro.z > v3AbsGyro.y) 
            {
                if (v3AbsAttitude.x < 10 - gestureProperties.Tolerance / 3 && v3AbsAttitude.y < 10 - gestureProperties.Tolerance / 3) 
                {
                    // case 3. 바닥에 두고 반시계 방향 회전
                    if (v3Gyro.z > gestureProperties.AngularForce)
                    {
                        int i = (int) eGestureMotion.RotateCCW;

                        if (percent[i] < (v3Gyro.z * 100f) / gestureProperties.AngularForce)
                            percent[i] = (v3Gyro.z * 100f) / gestureProperties.AngularForce;

                        counter[i] ++;
                    }

                    // case 4. 바닥에 두고 시계 방향 회전
                    else if (v3Gyro.z < (gestureProperties.AngularForce * -1))
                    {
                        int i = (int) eGestureMotion.RotateCW;

                        if (percent[i] < (v3Gyro.z * 100f) / gestureProperties.AngularForce)
                            percent[i] = (v3Gyro.z * 100f) / gestureProperties.AngularForce;

                        counter[i] ++;
                    }
                }
                else 
                {
                    // case N. 손에 들고 Z축 회전
                    int i = (int) eGestureMotion.Other;

                    if (percent[i] < (v3Gyro.z * 100f) / gestureProperties.AngularForce)
                        percent[i] = (v3Gyro.z * 100f) / gestureProperties.AngularForce;

                    counter[i] ++;
                }
            }
        }

        private void ExpressGesture() 
        {
            bool nonZero = false;
            for (int i = 0; i < counter.Count; i++) 
            {
                if (counter[i] != 0) 
                {
                    nonZero = true;
                    break;
                }
            }

            if (nonZero == false) 
            {
                _txtLog.text = "입력된 동작이 없습니다.";
                _btnReDetect.interactable = true;
                return;
            }

            float maxPercent = percent.Max();
            int maxPercentIndex = percent.IndexOf(maxPercent);
            eGestureMotion p = (eGestureMotion) maxPercentIndex;

            int maxCounter = counter.Max();
            int maxCounterIndex = counter.IndexOf(maxCounter); 
            eGestureMotion c = (eGestureMotion) maxCounterIndex;

            int act = actGesture[maxCounterIndex];
            eGestureAct a = (eGestureAct) act;

            builder.Append("<< 결과 값 >> \n");
            builder.Append("Avatar Action : ");
            builder.Append(Util.EnumToString<eGestureAct>(a));
            builder.Append("\n\n가장 많은 동작 : ");
            builder.Append(Util.EnumToString<eGestureMotion>(c));
            builder.Append(", 횟수 : ");
            builder.Append(counter[maxCounterIndex]);
            builder.Append("회 \n");
            builder.Append("가장 큰 동작 : ");
            builder.Append(Util.EnumToString<eGestureMotion>(p));
            builder.Append(", 기준 대비 비율 : ");
            builder.Append(percent[maxPercentIndex].ToString("F2"));

            /*
            builder.Append("% \n\n");
            builder.Append("<< 상세 값 >> \n");

            eGestureMotion g;
            for (int i = 0; i < (int) eGestureMotion.LENGTH; i++) 
            {
                g = (eGestureMotion) i;
                builder.Append(Util.EnumToString<eGestureMotion>(g));
                builder.Append("\n - 동작 감지 횟수 : ");
                builder.Append(counter[i]);
                builder.Append("회");
                builder.Append("\n - 힘 기준치 초과 : ");
                builder.Append(percent[i].ToString("F2"));
                builder.Append("%\n");
            }

            builder.Append("\n\n");
            */
            _txtLog.text = builder.ToString();
            _btnReDetect.interactable = true;

            // StartCoroutine(SendMotion(act));
        }

        /**
        private IEnumerator SendMotion(int act) 
        {
            string token = Repo.Instance.token;
            int workspaceSeq = Repo.Instance.workspaceSeq;
            int memberSeq = Repo.Instance.memberSeq;

            string msg = string.Format("이모지알림_{0}_1_{1}", memberSeq, act);
            Debug.LogInfo("[gesture] msg : "+ msg);
            string url = string.Format(URL.URL_WORKSPACE_MEMBER_LIST, workspaceSeq);
            Debug.LogInfo("[gesture] url : "+ url);

            bool onError = false;
            WorkspaceMemberList list = null;
            yield return StartCoroutine(API.Instance.Get(url, token, 
                (WorkspaceMemberList complete) => {
                    list = complete;
                    Debug.LogInfo("[gesture] get list complete.");
                },
                (long code, string error) => {
                    onError = true;

                    _txtLog.text = string.Format("{0}, error : {1}", "멤버 리스트를 가져오지 못했습니다.", error);
                    _btnReDetect.interactable = true;
                },
                true
            ));

            if (onError) yield break;
            Debug.LogInfo("[gesture] get list count : "+ list.list.Count);

            SpaceInfo spaceInfo = null;
            int topSeq = 0;
            string xmppUrl = "";
            foreach (WorkspaceMemberInfo info in list.list) 
            {
                // offline check. my seq check
                Debug.LogInfo("[gesture] nm : "+ info.nickNm +", seq : "+ info.seq + ", status id : "+ info.status.id);
                if (info.status.id.Equals(Strings.OFFLINE) || info.seq == memberSeq) continue;

                // other avatar : current position check
                Debug.LogInfo("[gesture] space seq : "+ info.cspace.seq);
                if (currSpaceTopSeq == 0) 
                {
                    if (info.cspace.seq != currSpaceSeq) continue;
                }
                else 
                {
                    // get space top seq
                    foreach (var space in SpaceManager.Instance.listAll) 
                    {
                        spaceInfo = space.GetComponent<SpaceInfo>();
                        if (spaceInfo.spaceSeq == info.cspace.seq) 
                        {
                            topSeq = spaceInfo.topSpace.nm.Equals("") ? spaceInfo.spaceSeq : spaceInfo.topSpace.seq;
                            break;
                        }
                    }

                    Debug.LogInfo("[gesture] top space seq : "+ topSeq);
                    if (topSeq != currSpaceTopSeq) continue;
                }

                // send xmpp
                xmppUrl = string.Format(URL.URL_SEND_XMPP, msg, info.seq, 0, workspaceSeq);
                Debug.LogInfo("[gesture] xmpp url : "+ xmppUrl);

                StartCoroutine(API.Instance.Post(xmppUrl, token, "xmpp", 
                    (long code, string complete) => {
                        Debug.LogInfo("정상적으로 전달. to : "+ info.seq);
                        if (myAvatar != null) 
                        {
                            myAvatar.StartAvatarAnimation(act);
                        }
                    },
                    (long code, string error) => {
                        Debug.LogInfo("XMPP 전달 실패. to : "+ info.seq);
                    },
                    true
                ));
            }

            _btnReDetect.interactable = true;
        }
         */
    #endregion  // Gesture detection


    #region event handling
        private async UniTask<int> Refresh()
        {
            // navigation control
            MobileManager.singleton.ShowNavigation(true);

            // gps control
            // _toggleGpsOff.isOn = ! GpsManager.Instance.isRun;
            // _goGpsInfo.SetActive(GpsManager.Instance.isRun);

            // gesture control
            _goGestureGuide.SetActive(false);
            InitGestureDetector();

            await UniTask.Yield();
            return 0;
        }

        private void BackButtonProcess(string name="") 
        {
            if (! name.Equals(gameObject.name)) return;
            if (visibleState != eVisibleState.Appeared) return;

            BackProcess();
        }

        private void BackProcess() 
        {
            Debug.Log("종료 여부 확인");
        }
    #endregion  // event handling
    }
}