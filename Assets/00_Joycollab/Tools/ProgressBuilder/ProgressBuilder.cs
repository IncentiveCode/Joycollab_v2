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
        public static ProgressBuilder Instance; 

        [SerializeField, Tooltip("for test")] private Transform canvas;
        // TODO. 추후 프로그레스 바도 제어할 수 있도록 만들 예정.
        // [SerializeField] private GameObject _goProgressBar;
        [SerializeField] private GameObject _goProgressDialog;


    #region Unity functions
        private void Awake() 
        {
            Instance = this;
            canvas = GameObject.Find(S.POPUP_CANVAS).GetComponent<Transform>();
        }
    #endregion  // Unity functions


    #region Public functions
        public ProgressDialog Build() 
        {
            if (_goProgressDialog == null || canvas == null) return null;

            var view = Instantiate(_goProgressDialog, Vector3.zero, Quaternion.identity);
            var lib = view.GetComponent<ProgressDialog>();
            view.transform.SetParent(canvas, false);
            return lib;
        }
    #endregion  // Public functions
    }
}