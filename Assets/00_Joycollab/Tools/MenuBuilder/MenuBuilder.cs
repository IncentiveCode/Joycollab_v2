/// <summary>
/// 여기저기 떨어져 있는 메뉴 생성 함수를 하나로 묶기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 10. 06 
/// @version        : 0.2
/// @update
///     v0.1 (2023. 03. 06) : 최초 생성
///     v0.2 (2023. 10. 06) : 기능 수정 및 테스트 진행
/// </summary>

using UnityEngine;

namespace Joycollab.v2
{
    public class MenuBuilder : MonoBehaviour
    {
        private const string TAG = "MenuBuilder";

        [Header("object")]
        [SerializeField] private Transform _transform;
        [SerializeField] private GameObject _goMenu;

        [Header("tag")]
        [TagSelector] 
        [SerializeField] private string viewTag;

        public static MenuBuilder singleton { get; private set; }


    #region Unity function

        private void Awake() 
        {
            singleton = this;
        }

    #endregion  // Unity function


    #region Private function

        private void SetTransform() 
        {
        #if UNITY_ANDROID || UNITY_IOS
            _transform = GameObject.Find(S.Canvas_Popup_M).GetComponent<Transform>();
        #else
            _transform = GameObject.Find(S.Canvas_Menu).GetComponent<Transform>();
        #endif
        }

        private void Clear() 
        {
            if (_transform == null) SetTransform();

            foreach (Transform child in _transform.GetComponentInChildren<Transform>()) 
            {
                if (child.name.Equals(_transform.name) || child.GetComponent<MenuController>() == null) continue;
                Destroy(child.gameObject);
            }
        }

    #endregion  // Private function


    #region Public function

        public MenuController Build() 
        {
            if (_goMenu == null) return null;
            if (_transform == null) SetTransform();

            var view = Instantiate(_goMenu, Vector3.zero, Quaternion.identity);
            var lib = view.GetComponent<MenuController>();
            view.transform.SetParent(_transform, false);
            return lib;
        }

        public void RequestClear() => Clear();

        public int GetMenuCount()
        {
            if (_transform == null) SetTransform();
            return _transform.childCount;
        }

    #endregion  // Public function
    }
}