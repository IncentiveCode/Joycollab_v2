/// <summary>
/// Webview 출력을 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 06. 16
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 16) : 최초 생성, gree webview 적용.
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class WebviewBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject _goWebview;
        [SerializeField] private Transform _transform;

        public static WebviewBuilder singleton { get; private set; }


    #region Unity functions

        private void Awake() 
        {
            InitSingleton();
        }

    #endregion  // Unity functions


    #region Public function

        public void RequestClear() => Clear();

        public bool Active() 
        {
            if (_transform == null) SetTransform();
            return _transform.childCount > 0;
        }

        public WebviewController Build() 
        {
            if (_goWebview == null) return null;
            if (_transform == null) SetTransform();

            var view = Instantiate(_goWebview, Vector3.zero, Quaternion.identity);
            var lib = view.GetComponent<WebviewController>();
            view.transform.SetParent(_transform, false);
            return lib;
        }

        public void ShowMobileWebview(string url) 
        {
            WebviewController ctrl = Build(); 
            ctrl.ShowMobileWebview(url);
        }

    #endregion


    #region Private functions

        private void InitSingleton() 
        {
            if (singleton != null && singleton == this) return;
            if (singleton != null) 
            {
                Destroy(gameObject);
                return;
            }

            singleton = this;
            DontDestroyOnLoad(gameObject);
        }

        private void SetTransform() 
        {
        #if UNITY_ANDROID || UNITY_IOS
            _transform = GameObject.Find(S.Canvas_Webview_M).GetComponent<Transform>();
        #else
            _transform = GameObject.Find(S.Canvas_Webview).GetComponent<Transform>();
        #endif
        }

        private void Clear()
        {
            if (_transform == null) SetTransform();

            foreach (Transform child in _transform.GetComponentInChildren<Transform>())
            {
                if (child.name.Equals(_transform.name) || child.GetComponent<WebviewController>() == null) continue;
                Destroy(child.gameObject);
            }
        }

    #endregion  // Private functions
    }
}