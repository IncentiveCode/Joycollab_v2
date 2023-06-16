/// <summary>
/// 여기저기 떨어져 있는 팝업 생성 함수를 하나로 묶기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 06. 12
/// @version        : 0.6
/// @update
///     v0.1 (2023. 02. 09) : TP 에서 작업했던 내용을 가지고 와서 편집.
///     v0.2 (2023. 03. 30) : 추가 정리 및 예시 기술.
///     v0.3 (2023. 04. 07) : mobile 과 web 에서 함께 사용할 수 있도록 구성.
///     v0.4 (2023. 04. 13) : singleton pattern 수정.
///     v0.5 (2023. 04. 17) : public function (for alert, confirm) 추가.
///     v0.6 (2023. 06. 12) : GetPopupCount, Clear function 에서 transform 을 못 찾는 부분이 있어서 함수 수정.
/// </summary>

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Joycollab.v2
{
    public class PopupBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject _goPopup;
        [SerializeField] private GameObject _goSlidePopup;
        [SerializeField] private Transform _transform;

        public static PopupBuilder singleton { get; private set; }


    #region Unity functions

        private void Awake() 
        {
            InitSingleton();
        }

    #endregion


    #region Public function

        public void RequestClear() => Clear();

        public int GetPopupCount() 
        {
            if (_transform == null) SetTransform();
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
        }

        private void SetTransform() 
        {
        #if UNITY_ANDROID || UNITY_IOS
            _transform = GameObject.Find(S.Canvas_Popup_M).GetComponent<Transform>();
        #else
            _transform = GameObject.Find(S.Canvas_Popup).GetComponent<Transform>();
        #endif
        }
        
        private PopupController Build() 
        {
        #if UNITY_ANDROID || UNITY_IOS
            if (_goSlidePopup == null) return null;
        #endif
            if (_goPopup == null) return null;
            if (_transform == null) SetTransform();

            var view = Instantiate(_goPopup, Vector3.zero, Quaternion.identity);
            var lib = view.GetComponent<PopupController>();
            view.transform.SetParent(_transform, false);
            return lib;
        }

        private void Clear()
        {
            if (_transform == null) SetTransform();

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
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            string notice = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "알림", currentLocale);
            string confirm = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "확인", currentLocale);

            PopupController ctrl = Build();
            ctrl.Type = ePopupType.Alert;

            string t = string.IsNullOrEmpty(title) ? notice : title;
            ctrl.Title = t; 
            ctrl.Content = content;

            t = string.IsNullOrEmpty(btnText) ? confirm : btnText;
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
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            string notice = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "알림", currentLocale);
            string confirm = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "확인", currentLocale);
            string cancel = LocalizationSettings.StringDatabase.GetLocalizedString("Texts", "취소", currentLocale);

            PopupController ctrl = Build();
            ctrl.Type = ePopupType.Confirm;

            string t = string.IsNullOrEmpty(title) ? notice : title;
            ctrl.Title = t;
            ctrl.Content = content;

            t = string.IsNullOrEmpty(yesText) ? confirm : yesText;
            ctrl.AddButton(ePopupButtonType.Normal, t, () => yesAction?.Invoke());

            t = string.IsNullOrEmpty(noText) ? cancel : noText;
            ctrl.AddButton(ePopupButtonType.Warning, t, () => noAction?.Invoke());

            ctrl.Open();
        }

    #endregion  // Confirm functions


    #region Slide popup 

        public void OpenSlide(string title, string[] options, string[] extra, int viewID, bool cancelable=false) 
        {
            if (_transform == null) SetTransform();
            
            AndroidSelectCallback.ViewID = viewID;
            AndroidSelectCallback.extraData.Clear();
            foreach (string s in extra) 
            {
                AndroidSelectCallback.extraData.Add(s);
            }
            AndroidSelectCallback.isUpdated = false;

            var popup = Instantiate(_goSlidePopup, Vector3.zero, Quaternion.identity);
            SlidePopupM sc = popup.GetComponent<SlidePopupM>();
            sc.InitPopup(title, options, cancelable);
            sc.transform.SetParent(_transform, false);
        }

    #endregion  // Slide popup
    }
}