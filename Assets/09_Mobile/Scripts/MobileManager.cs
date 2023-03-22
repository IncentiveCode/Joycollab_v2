/// <summary>
/// Mobile Scene Manager class
/// @author         : HJ Lee
/// @last update    : 2023. 03. 21
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 21) : Joycollab 에서 사용하던 클래스 정리 및 통합
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joycollab.v2
{
    public class MobileManager : MonoBehaviour
    {
        private const string TAG = "MobileManager";

        // singleton
        public static MobileManager singleton { get; private set; }

        [Header("navigation")]
        [SerializeField] private NaviTopM _naviTop;
        [SerializeField] private NaviBottomM _naviBottom;
        
        // for UI stack
        private Dictionary<string, FixedView> dictViews;
        private GameObject[] arrViews;
        private Stack uiNavigation;
        private FixedView currentView;


    #region Unity functions
        private void Awake() 
        {
            // TEST
            // PlayerPrefs.DeleteAll();

            singleton = this;
        }

        private void Start() 
        {
        #if UNITY_ANDROID && !UNITY_EDITOR
            Rect safeArea = Screen.safeArea;
            bool hasNotch = safeArea.height != Screen.height;
            /**
            if (hasNotch) 
            {
             */
                ApplicationChrome.statusBarColor = 0xfffeb336;
            /**
            }
             */

            // Android 의 경우 status bar 출력
            ApplicationChrome.statusBarState = ApplicationChrome.States.Visible;
            ApplicationChrome.navigationBarState = ApplicationChrome.States.Visible;
        #endif


            // init UI stack
            dictViews = new Dictionary<string, FixedView>();
            arrViews = GameObject.FindGameObjectsWithTag(S.MobileScene_ViewTag);
            foreach (GameObject obj in arrViews) 
            {
                dictViews.Add(obj.name, obj.GetComponent<FixedView>());
            }

            uiNavigation = new Stack();
            currentView = null;

            // TODO. 임시로 login 창을 출력함.
            Push(S.MobileScene_Login);
        }
    #endregion  // Unity functions


    #region UI stack, Navigation
        public void ShowNavigation(bool on) 
        {
            _naviTop.ShowNavigation(on);
            _naviBottom.ShowNavigation(on);
        }

        public void ShowBottomNavigation() 
        {
            _naviTop.ShowNavigation(false);
            _naviBottom.ShowNavigation(true);
        }

        public void StartOnMySeat(bool on) 
        {
            _naviBottom.StartOnMySeat(on);
        }

        public void Push(string viewName, string optionName="")
        {
            if (dictViews.ContainsKey(viewName)) 
            {
                FixedView view = dictViews[viewName];
                if (currentView != null) 
                { 
                    uiNavigation.Push(currentView);
                    currentView.Hide();
                }

                currentView = view;
                if (string.IsNullOrEmpty(optionName))
                    currentView.Show().Forget();
                else
                    currentView.Show(optionName).Forget();
            }
            else 
            {
                Debug.Log($"{TAG} | Push - 잘못된 UIView 이름 : "+ viewName);
            }
        }

        public void Overlay(string viewName, string optionName="") 
        {
            if (dictViews.ContainsKey(viewName)) 
            {
                FixedView view = dictViews[viewName];
                if (currentView != null) 
                {
                    uiNavigation.Push(currentView);
                }

                currentView = view;
                if (string.IsNullOrEmpty(optionName))
                    currentView.Show().Forget();
                else
                    currentView.Show(optionName).Forget();
            }
            else 
            {
                Debug.Log($"{TAG} | Overlay - 잘못된 UIView 이름 : "+ viewName);
            }
        }

        public void Pop(string optionName="") 
        {
            if (uiNavigation.Count == 0) return;

            FixedView previous = uiNavigation.Pop() as FixedView;
            if (previous != null) 
            {
                if (currentView != null) 
                {
                    currentView.Hide();
                }

                currentView = previous;
                if (string.IsNullOrEmpty(optionName))
                    currentView.Show().Forget();
                else
                    currentView.Show(optionName).Forget();
            }
        }

        public void CloseOverlay() 
        {
            if (uiNavigation.Count == 0) return;
            FixedView previous = uiNavigation.Pop() as FixedView;
            if (previous != null)
            {
                if (currentView != null)
                {
                    currentView.Hide();
                }
                currentView = previous;
            }
        }

        public void PopAll() 
        {
            FixedView view = null;
            while (uiNavigation.Count > 0) 
            {
                view = uiNavigation.Pop() as FixedView;
                if (view != null) view.Hide();
            }
        }

        public void PopToRoot() 
        {
            while (uiNavigation.Count > 1) 
            {
                uiNavigation.Pop();
            }

            FixedView root = uiNavigation.Pop() as FixedView;
            if (root != null) 
            {
                if (currentView != null) 
                    currentView.Hide();

                currentView = root;
                currentView.Show().Forget();
            }
        }

        public string GetTopViewName() 
        {
            if (currentView != null)
            {
                return currentView.name; 
            }
            else 
            {
                return string.Empty;
            }
        }

        public int GetStackCount()
        {
            return uiNavigation.Count;
        }
    #endregion  // UI stack, Navigation
    }
}