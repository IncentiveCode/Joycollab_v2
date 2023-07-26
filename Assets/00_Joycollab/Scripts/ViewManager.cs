/// <summary>
/// 각 Scene 에서 화면을 관리하는 manager class
/// @author         : HJ Lee
/// @last update    : 2023. 07. 26
/// @version        : 0.6
/// @update
///     v0.1 (2023. 05. 04) : 최초 작성, 기존 Manager 에서 View 관리 부분만 빼서 생성.
///     v0.2 (2023. 05. 11) : android 의 back button 처리 추가.
///     v0.3 (2023. 06. 13) : android part 는 AndroidLib 로 분리.
///     v0.4 (2023. 07. 21) : int seq 를 넘기는 Push(), Pop() 함수 추가.
///     v0.5 (2023. 07. 24) : 화면 초기화 [ Init() ]를 async 로 구성.
///     v0.6 (2023. 07. 26) : Push(bool refresh) 추가. Init() 에서 top/bottom navigation 찾는 기능 수정.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

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

        private async UniTaskVoid Start() 
        {
            int res = await Init();
            if (res == -1 || _goTop == null || _goBottom == null) 
            {
                PopupBuilder.singleton.OpenAlert("ViewManager 초기화 실패.");
                return;
            }

        #if UNITY_ANDROID || UNITY_IOS
            Push(S.MobileScene_Login);
        #else
            Push(S.LoginScene_Login);
        #endif

            isDone = true;
        }

        #if UNITY_ANDROID
        private void Update() 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                string name = GetTopViewName();
                MobileEvents.singleton.BackButtonProcess(name);
            }
        }
        #endif

    #endregion  // Unity functions


    #region UI Stack

        private Dictionary<string, FixedView> dictViews;
        private Stack uiNavigation;
        private FixedView currentView;

        private async UniTask<int> Init() 
        {
            // 0. init navigation
            GameObject[] navigations = GameObject.FindGameObjectsWithTag(S.MobileNaviTag);
            foreach (GameObject o in navigations) 
            {
                if (o.TryGetComponent<TopM>(out TopM top)) 
                {
                    _goTop = top;
                }
                else if (o.TryGetComponent<BottomM>(out BottomM bottom))
                {
                    _goBottom = bottom;
                }
            }
            await UniTask.WaitUntil(() => _goTop != null && _goBottom != null);

            // 1. init ui stack
            dictViews = new Dictionary<string, FixedView>();
            GameObject[] arrViews = GameObject.FindGameObjectsWithTag(viewTag);
            foreach (GameObject obj in arrViews) 
            {
                dictViews.Add(obj.name, obj.GetComponent<FixedView>());
            }
            uiNavigation = new Stack();
            currentView = null;

            return (dictViews.Count == 0) ? -1 : 0;
        }

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

        public void Push(string viewName, int seq) 
        {
            Debug.Log($"{TAG} | Push(), inserted view : {viewName}, seq : {seq}");

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
            if (seq == 0)
                currentView.Show().Forget();
            else
                currentView.Show(seq).Forget();
        }

        public void Push(string viewName, bool refresh) 
        {
            Debug.Log($"{TAG} | Push(), inserted view : {viewName}, refresh : {refresh}");

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
            currentView.Show(refresh).Forget();
        }

        public void PushTest(string viewName) 
        {
            Debug.Log($"{TAG} | PushTest(), inserted view : {viewName}");

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
            currentView.Show().Forget();
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

        public void Pop(int seq) 
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
            if (seq == 0)
                currentView.Show().Forget();
            else
                currentView.Show(seq).Forget();
        }

        public void Pop(bool refresh) 
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
            currentView.Show(refresh).Forget();
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
                currentView.Block(false);
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
                Debug.Log($"{TAG} | CloseOverlay(), previous view is null. return.");
                return;
            }

            if (currentView != null)
            {
                currentView.Hide();
            }

            currentView = previous;
            currentView.Block(true);
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