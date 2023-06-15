/// <summary>
/// 프로그레스 바/다이얼로그 생성 툴
/// @author         : HJ Lee
/// @last update    : 2023. 03. 07 
/// @version        : 1.0
/// @update
///     - v1.0 (2023. 03. 07) : 최초 생성
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class ProgressBuilder : MonoBehaviour
    {
        // TODO. 추후 프로그레스 바도 제어할 수 있도록 만들 예정.
        // [SerializeField] private GameObject _goProgressBar;
        [SerializeField] private GameObject _goProgressDialog;
        [SerializeField] private Transform _transform;

        public static ProgressBuilder singleton { get; private set; } 


    #region Unity functions

        private void Awake() 
        {
            InitSingleton();
        }

    #endregion  // Unity functions


    #region Public functions

        public void OpenProgress(float timer) => OpenProgress(timer, null);
        public void OpenProgress(float timer, System.Action action) 
        {
            ProgressDialog dialog = Build();
            dialog.Open(timer, action);
        }

    #endregion  // Public functions


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
            return;
        }

        private ProgressDialog Build() 
        {
            if (_goProgressDialog == null) return null;

            if (_transform == null) 
            {
        #if UNITY_ANDROID || UNITY_IOS
                _transform = GameObject.Find(S.Canvas_Progress_M).GetComponent<Transform>();
        #endif
            }

            var view = Instantiate(_goProgressDialog, Vector3.zero, Quaternion.identity);
            var lib = view.GetComponent<ProgressDialog>();
            view.transform.SetParent(_transform, false);
            return lib;
        }

        private void Clear() 
        {
            if (_transform == null) 
            {
        #if UNITY_ANDROID || UNITY_IOS
                _transform = GameObject.Find(S.Canvas_Progress_M).GetComponent<Transform>();
        #endif
            }

            foreach (Transform child in _transform.GetComponentInChildren<Transform>())
            {
                if (child.name.Equals(_transform.name) || child.GetComponent<ProgressDialog>() == null) continue;
                Destroy(child.gameObject);
            }
        }

    #endregion  // Private functions
    }
}