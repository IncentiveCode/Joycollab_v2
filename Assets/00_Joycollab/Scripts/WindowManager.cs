/// <summary>
/// 각 Scene 에서 Window 을 관리하는 manager class
/// @author         : HJ Lee
/// @last update    : 2023. 09. 14
/// @version        : 0.8
/// @update
///     v0.1 (2023. 09. 14) : 최초 작성, ViewManager 를 참고해서 생성.
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Joycollab.v2 
{
    [Serializable]
    public class SerializedWindow 
    {
        public string nm;
        public GameObject go;
    }

    public class WindowManager : MonoBehaviour
    {
        public static WindowManager singleton { get; private set; }
        private const string TAG = "WindowManager";

        [SerializeField] private Transform _transform;
        [SerializeField] private List<SerializedWindow> _dictWindows;


    #region Unity functions

        private void Awake()
        {
            singleton = this;
            dictViews = new Dictionary<string, WindowView>();
            tempView = null;

            if (_transform == null) 
                _transform = GameObject.Find(S.Canvas_Master).GetComponent<Transform>();
        }

    #endregion  // Unity functions


    #region UI Stack

        private Dictionary<string, WindowView> dictViews;
        private WindowView tempView;

        public void Push(string viewName) 
        {
            Debug.Log($"{TAG} | Push(), request view : {viewName}");

            // 0. Get game object
            GameObject go = null;
            foreach (var t in _dictWindows) 
            {
                if (t.nm.Equals(viewName)) 
                {
                    go = t.go;
                    break;
                }
            }
            if (go == null) 
            {
                Debug.Log($"{TAG} | Push(), _dictWindow 안에 같은 이름을 가진 Object 가 없음.");
                return;
            }

            // 1. check dictionary
            bool inDictionary = dictViews.ContainsKey(viewName);
            if (! inDictionary) 
            {
                Debug.Log($"{TAG} | Push(), 새로 생성 : {viewName}");

                var obj = Instantiate(go, Vector3.zero, Quaternion.identity);
                obj.transform.SetParent(_transform, false);

                tempView = obj.GetComponent<WindowView>();
                dictViews.Add(viewName, tempView);

                tempView.Show().Forget();
            }
            else 
            {
                tempView = dictViews[viewName];
                if (tempView.AllowMultiple)
                {
                    Debug.Log($"{TAG} | Push(), 이미 있지만 새로 생성 : {viewName}");

                    // TODO. view name 을 loop 돌려가며 index 추가.

                    // TODO. 새 이름과 함께 window instantiate 추가.
                    tempView.Show().Forget();
                }
                else
                {
                    Debug.Log($"{TAG} | Push(), 중복 생성을 허용하지 않는 창 이기에, 생성하지 않고 기존 창을 활성화.");
                    if (tempView.isDisappeared)
                        tempView.Show().Forget();
                    else
                        tempView.SetAsLastSibling();
                }
            }
        }

    #endregion  // UI Stack
    }
}