/// <summary>
/// 각 Scene 에서 화면을 관리하는 manager class
/// @author         : HJ Lee
/// @last update    : 2023. 05. 04
/// @version        : 0.1
/// @update
///     v0.1 (2023. 05. 04) : 최초 작성, 기존 Manager 에서 View 관리 부분만 빼서 생성.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joycollab.v2 
{
    public class ViewManager : MonoBehaviour
    {
        public static ViewManager singleton { get; private set; }
        private const string TAG = "ViewManager";

        [TagSelector]
        [SerializeField] private string viewTag;
        private bool isDone;


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