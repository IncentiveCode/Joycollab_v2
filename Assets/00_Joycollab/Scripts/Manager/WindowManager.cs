/// <summary>
/// 각 Scene 에서 Window 을 관리하는 manager class
/// @author         : HJ Lee
/// @last update    : 2023. 11. 02
/// @version        : 0.2
/// @update
///     v0.1 (2023. 09. 14) : 최초 작성, ViewManager 를 참고해서 생성.
///     v0.2 (2023. 11. 02) : Push(string viewName, int seq, float x, float y) 추가.
///                           tempView.AllowMultiple 는 테스트가 조금 더 필요해보임.
/// </summary>

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2 
{
    [Serializable]
    public class SerializedWindow 
    {
        public string name;
        public GameObject gameObject;
    }

    public class WindowManager : MonoBehaviour
    {
        public static WindowManager singleton { get; private set; }
        private const string TAG = "WindowManager";

        [SerializeField] private Transform _transform;
        [SerializeField] private List<SerializedWindow> _dictWindows;

        // canvas scaler, ratio
        private CanvasScaler scaler;
        private float ratio;


    #region Unity functions

        private void Awake()
        {
            singleton = this;
            dictViews = new Dictionary<string, WindowView>();
            tempView = null;

            if (! GameObject.Find(S.Canvas_Master).TryGetComponent<Transform>(out _transform)) 
            {
                Debug.Log($"{TAG} | transform 획득 실패.");
                return;
            }

            if (! _transform.TryGetComponent<CanvasScaler>(out scaler))
            {
                Debug.Log($"{TAG} | canvas scaler 획득 실패.");
                return;
            }
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
                if (t.name.Equals(viewName)) 
                {
                    go = t.gameObject;
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

        public void Push(string viewName, int seq, float posX=0f, float posY=0f) 
        {
            Debug.Log($"{TAG} | Push(seq), request view : {viewName}, seq : {seq}, pos x : {posX}, pos y : {posY}");

            // 0. Get game object
            GameObject go = null;
            foreach (var t in _dictWindows) 
            {
                if (t.name.Equals(viewName)) 
                {
                    go = t.gameObject;
                    break;
                }
            }
            if (go == null) 
            {
                Debug.Log($"{TAG} | Push(), _dictWindow 안에 같은 이름을 가진 Object 가 없음.");
                return;
            }

            // 0. organize name
            string objectName = $"{viewName}_{seq}";

            // 1. check dictionary
            bool inDictionary = dictViews.ContainsKey(objectName);
            if (! inDictionary) 
            {
                Debug.Log($"{TAG} | Push(), 새로 생성 : {objectName}");

                var obj = Instantiate(go, Vector3.zero, Quaternion.identity);
                obj.transform.SetParent(_transform, false);
                obj.name = objectName;
                tempView = obj.GetComponent<WindowView>();
                dictViews.Add(objectName, tempView);

                tempView.Show(seq, new Vector2(posX, posY)).Forget();
            }
            else 
            {
                tempView = dictViews[objectName];
                if (tempView.AllowMultiple)
                {
                    Debug.Log($"{TAG} | Push(), 이미 있지만 새로 생성 : {objectName}");
                    var obj = Instantiate(go, Vector3.zero, Quaternion.identity);
                    obj.transform.SetParent(_transform, false);
                    obj.name = objectName;

                    // TODO. 새 이름과 함께 window instantiate 추가.
                    tempView.Show(seq, new Vector2(posX, posY)).Forget();
                }
                else
                {
                    tempView.Show(seq, new Vector2(posX, posY)).Forget();
                    tempView.SetAsLastSibling();
                }
            }
        }

    #endregion  // UI Stack


    #region Utilities

        public Vector2 CalculatePosition(Vector2 pos, Vector2 size) 
        {
            float ratio = Util.CalculateScalerRatio(scaler);
            Vector2 v2Temp = (pos / ratio);

            float mX = (float) (Screen.width / ratio);
            float mY = (float) (Screen.height / ratio);
            float pX = size.x;
            float pY = size.y;

            v2Temp.x = Mathf.Clamp(v2Temp.x, 0, mX - pX);
            v2Temp.y = Mathf.Clamp(v2Temp.y, 0, mY - pY);

            return v2Temp;
        }

    #endregion  // Utilities
    }
}