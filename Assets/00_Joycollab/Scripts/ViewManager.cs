/// <summary>
/// 각 Scene 에서 화면을 관리하는 manager class
/// @author         : HJ Lee
/// @last update    : 2023. 05. 11
/// @version        : 0.2
/// @update
///     v0.1 (2023. 05. 04) : 최초 작성, 기존 Manager 에서 View 관리 부분만 빼서 생성.
///     v0.2 (2023. 05. 11) : android 의 back button 처리 추가.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2 
{
    public class ViewManager : MonoBehaviour
    {
        public static ViewManager singleton { get; private set; }
        private const string TAG = "ViewManager";

        [TagSelector]
        [SerializeField] private string viewTag;
        private bool _isDone;
        public bool isDone {
            get { return _isDone; }
            set { _isDone = value; }
        }


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
        private Text txtTarget;
    
    #endif  // for android plugin


    #if UNITY_ANDROID || UNITY_IOS  // for navigation bar

        [Header("Mobile navigation")]
        [SerializeField] private TopM _goTop;
        [SerializeField] private BottomM _goBottom;

        public void ShowNavigation(bool on) 
        {
            _goTop.ShowNavigation(on);
            _goBottom.ShowNavigation(on);
        }

        public void ShowBottomNavigation() 
        {
            _goTop.ShowNavigation(false);
            _goBottom.ShowNavigation(true);
        }

        public void StartOnMySeat(bool on) 
        {
            _goBottom.StartOnMySeat(on);
        }
    
    #else

        public void ShowNavigation(bool on) { }
        public void ShowBottomNavigation() { }
        public void StartOnMySeat(bool on) { }

    #endif  // for navigation bar



    #region Unity functions

        private void Awake()
        {
            singleton = this;

            isDone = false;
        }

        private void Start() 
        {
            // init UI Stack
            dictViews = new Dictionary<string, FixedView>();
            arrViews = GameObject.FindGameObjectsWithTag(viewTag);
            foreach (GameObject obj in arrViews) 
            {
                dictViews.Add(obj.name, obj.GetComponent<FixedView>());
            }
            uiNavigation = new Stack();
            currentView = null;

            isDone = true;

            // test
        #if UNITY_ANDROID || UNITY_IOS
            Push(S.MobileScene_Login);
        #else
            Push(S.LoginScene_Login);
        #endif
        }

        #if UNITY_ANDROID
        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                string name = GetTopViewName();
                MobileEvents.singleton.BackButtonProcess(name);
            }

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


    #region UI Stack

        private Dictionary<string, FixedView> dictViews;
        private GameObject[] arrViews;
        private Stack uiNavigation;
        private FixedView currentView;

        public void Push(string viewName, string option="") 
        {
            Debug.Log($"{TAG} | Push(), inserted view : {viewName}, option : {option}");

            bool inDictionary = dictViews.ContainsKey(viewName);
            if (! inDictionary) 
            {
                Debug.Log($"{TAG} | Push(), 잘못된 View 이름 : {viewName}");
                return;
            }

            if (currentView != null) 
            {
                uiNavigation.Push(currentView);
                currentView.Hide();
            }

            currentView = dictViews[viewName];
            if (string.IsNullOrEmpty(option))
                currentView.Show().Forget();
            else
                currentView.Show(option).Forget();
        }

        public void Pop(string option="") 
        {
            if (uiNavigation.Count <= 0) 
            {
                Debug.Log($"{TAG} | Pop(), uiNavigation count is zero. return.");
                return;
            }

            FixedView previous = uiNavigation.Pop() as FixedView;
            if (previous == null) 
            {
                Debug.Log($"{TAG} | Pop(), previous view is null. return.");
                return;
            }

            if (currentView != null) 
            {
                currentView.Hide(); 
            }

            currentView = previous;
            if (string.IsNullOrEmpty(option))
                currentView.Show().Forget();
            else
                currentView.Show(option).Forget();
        }

        public void Overlay(string viewName, string option="") 
        {
            Debug.Log($"{TAG} | Overlay(), inserted view : {viewName}, option : {option}");

            bool inDictionary = dictViews.ContainsKey(viewName);
            if (! inDictionary) 
            {
                Debug.Log($"{TAG} | Overlay(), 잘못된 View 이름 : {viewName}");
                return;
            }

            if (currentView != null) 
            {
                uiNavigation.Push(currentView);
            }

            currentView = dictViews[viewName];
            if (string.IsNullOrEmpty(option))
                currentView.Show().Forget();
            else
                currentView.Show(option).Forget();
        }

        public void CloseOverlay() 
        {
            if (uiNavigation.Count <= 0) 
            {
                Debug.Log($"{TAG} | CloseOverlay(), uiNavigation count is zero. return.");
                return;
            }

            FixedView previous = uiNavigation.Pop() as FixedView;
            if (previous == null) 
            {
                Debug.Log($"{TAG} | Overlay(), previous view is null. return.");
                return;
            }

            if (currentView != null)
            {
                currentView.Hide();
            }

            currentView = previous;
        }

        public void PopAll() => uiNavigation.Clear();

        public void PopToRoot() 
        {
            while (uiNavigation.Count > 1) 
            {
                uiNavigation.Pop();
            }

            FixedView root = uiNavigation.Pop() as FixedView;
            if (root == null) 
            {
                Debug.Log($"{TAG} | PopToRoot(), root view is null. return.");
                return;
            }

            if (currentView != null)
            {
                currentView.Hide();
            }

            currentView = root;
            currentView.Show().Forget();
        }

        public string GetTopViewName() 
        {
            return (currentView != null) ? currentView.name : string.Empty;
        }

        public int GetStackCount() 
        {
            return uiNavigation.Count;
        }

    #endregion  // UI Stack
    }
}