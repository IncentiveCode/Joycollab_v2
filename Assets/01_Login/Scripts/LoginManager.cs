/// <summary>
/// LoginScene 화면을 관리하는 manager class.
/// @author         : HJ Lee
/// @last update    : 2023. 05. 03
/// @version        : 0.6
/// @update
///     v0.1 : 최초 작성    
///     v0.2 : 새 기획 적용
///     v0.3 : UniTask 적용. Singleton<LoginViewManager> -> Monobehaviour 로 부모 클래스 변경
///     v0.4 (2023. 02. 08) : World 진입을 위한 parseUrl() 수정
///     v0.5 (2023. 03. 06) : 수정한 jslib 적용
///     v0.6 (2023. 05. 03) : Joycollab.v2 적용
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joycollab.v2
{
    public class LoginManager : MonoBehaviour
    {
        public static LoginManager singleton { get; private set; }
        private const string TAG = "LoginManager";


    #region Unity functions

        private void Awake()
        {
            singleton = this;
        }

        private void Start() 
        {
            // init UI Stack
            dictViews = new Dictionary<string, FixedView>();
            // arrViews = GameObject.FindGameObjectsWithTag(S.LoginScene_ViewTag);
            foreach (GameObject obj in arrViews) 
            {
                dictViews.Add(obj.name, obj.GetComponent<FixedView>());
            }
            uiNavigation = new Stack();
            currentView = null;


            // test
            Push(S.LoginScene_Login);
        }

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
                // TODO. 터치, 입력은 막는 함수 하나 추가할 것
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