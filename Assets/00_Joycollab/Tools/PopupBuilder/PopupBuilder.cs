/// <summary>
/// 여기저기 떨어져 있는 팝업 생성 함수를 하나로 묶기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 04. 17
/// @version        : 0.5
/// @update
///     v0.1 (2023. 02. 09) : TP 에서 작업했던 내용을 가지고 와서 편집.
///     v0.2 (2023. 03. 30) : 추가 정리 및 예시 기술.
///     v0.3 (2023. 04. 07) : mobile 과 web 에서 함께 사용할 수 있도록 구성.
///     v0.4 (2023. 04. 13) : singleton pattern 수정.
///     v0.5 (2023. 04. 17) : public function (for alert, confirm) 추가.
/// </summary>

using UnityEngine;
// using Assets.SimpleLocalization;

namespace Joycollab.v2
{
    public class PopupBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject _goPopup;
        [SerializeField] private Transform _transform;

        public static PopupBuilder singleton { get; private set; }


    #region Unity functions
        private void Awake() 
        {
            InitSingleton();

            if (_transform == null) 
            {
                // _transform = GameObject.Find(Strings.POPUP_CANVAS).GetComponent<Transform>();
                _transform = GameObject.Find("Popup Canvas").GetComponent<Transform>();
            }
        }
    #endregion


    #region Public function
        public PopupController Build() 
        {
            if (_transform == null || _goPopup == null) return null;

            var view = Instantiate(_goPopup, Vector3.zero, Quaternion.identity);
            var lib = view.GetComponent<PopupController>();
            view.transform.SetParent(_transform, false);
            return lib;
        }

        public void RequestClear() => Clear();

        public int GetPopupCount() 
        {
            return _transform.childCount;
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
            return;
        }

        private void Clear()
        {
            foreach (Transform child in _transform.GetComponentInChildren<Transform>())
            {
                if (child.name.Equals(_transform.name) || child.GetComponent<PopupController>() == null) continue;
                Destroy(child.gameObject);
            }
        }
    #endregion

    
    #region Alert functions
        public void OpenAlert(string content) => OpenAlert(string.Empty, content, string.Empty, null);
        public void OpenAlert(string content, System.Action action) => OpenAlert(string.Empty, content, string.Empty, action);
        public void OpenAlert(string title, string content) => OpenAlert(title, content, string.Empty, null);
        public void OpenAlert(string title, string content, System.Action action) => OpenAlert(title, content, string.Empty, action);
        public void OpenAlert(string title, string content, string btnText) => OpenAlert(title, content, btnText, null);
        public void OpenAlert(string title, string content, string btnText, System.Action action) 
        {
            PopupController ctrl = PopupBuilder.singleton.Build();
            ctrl.Type = ePopupType.Alert;

            string t = string.IsNullOrEmpty(title) ? "알림" : title;
            ctrl.Title = t; 
            ctrl.Content = content;

            t = string.IsNullOrEmpty(btnText) ? "확인" : btnText;
            ctrl.AddButton(ePopupButtonType.Normal, t, () => action?.Invoke());

            ctrl.Open();
        }
    #endregion  // Alert functions


    #region Confirm functions
        public void OpenConfirm(string content, System.Action yesAction) => OpenConfirm(string.Empty, content, string.Empty, yesAction, string.Empty, null);
        public void OpenConfirm(string content, System.Action yesAction, System.Action noAction) => OpenConfirm(string.Empty, content, string.Empty, yesAction, string.Empty, noAction);
        public void OpenConfirm(string title, string content, System.Action yesAction) => OpenConfirm(title, content, string.Empty, yesAction, string.Empty, null);
        public void OpenConfirm(string title, string content, System.Action yesAction, System.Action noAction) => OpenConfirm(title, content, string.Empty, yesAction, string.Empty, noAction);
        public void OpenConfirm(string title, string content, string yesText, System.Action yesAction) => OpenConfirm(title, content, yesText, yesAction, string.Empty, null);
        public void OpenConfirm(string title, string content, string yesText, System.Action yesAction, string noText) => OpenConfirm(title, content, yesText, yesAction, noText, null); 
        public void OpenConfirm(string title, string content, string yesText, System.Action yesAction, string noText, System.Action noAction) 
        {
            PopupController ctrl = PopupBuilder.singleton.Build();
            ctrl.Type = ePopupType.Confirm;

            string t = string.IsNullOrEmpty(title) ? "알림" : title;
            ctrl.Title = t;
            ctrl.Content = content;

            t = string.IsNullOrEmpty(yesText) ? "확인" : yesText;
            ctrl.AddButton(ePopupButtonType.Normal, t, () => yesAction?.Invoke());

            t = string.IsNullOrEmpty(noText) ? "취소" : noText;
            ctrl.AddButton(ePopupButtonType.Warning, t, () => noAction?.Invoke());

            ctrl.Open();
        }
    #endregion  // Confirm functions
    }
}