/// <summary>
/// 여기저기 떨어져 있는 팝업 생성 함수를 하나로 묶기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 08. 16 
/// @version        : 0.9
/// @update
///     v0.1 (2023. 02. 09) : TP 에서 작업했던 내용을 가지고 와서 편집.
///     v0.2 (2023. 03. 30) : 추가 정리 및 예시 기술.
///     v0.3 (2023. 04. 07) : mobile 과 web 에서 함께 사용할 수 있도록 구성.
///     v0.4 (2023. 04. 13) : singleton pattern 수정.
///     v0.5 (2023. 04. 17) : public function (for alert, confirm) 추가.
///     v0.6 (2023. 06. 12) : GetPopupCount, Clear function 에서 transform 을 못 찾는 부분이 있어서 함수 수정.
///     v0.7 (2023. 07. 29) : OpenConfirm() 의 기본 버튼을 확인/취소 에서 예/아니오 로 변경.
///     v0.8 (2023. 08. 11) : 버튼 생성시 tag 에 따라 login / world 스타일 분기점 추가.
///     v0.9 (2023. 08. 16) : Scene 마다 다르게 동작시키기 위해서, DontDestroyOnLoad() 제거.
/// </summary>

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Joycollab.v2
{
    public class PopupBuilder : MonoBehaviour
    {
        [Header("object")]
        [SerializeField] private GameObject _goPopup;
        [SerializeField] private GameObject _goSlidePopup;
        [SerializeField] private Transform _transform;

        [Header("tag")]
        [TagSelector]
        [SerializeField] private string viewTag;

        public static PopupBuilder singleton { get; private set; }


    #region Unity functions

        private void Awake() 
        {
            // InitSingleton();
            singleton = this;
        }

    #endregion


    #region Private functions

        /**
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
         */

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


    #region Public function

        public void RequestClear() => Clear();

        public int GetPopupCount() 
        {
            if (_transform == null) SetTransform();
            return _transform.childCount;
        }        

    
        #region Alert functions

        // - center 에 출력.
        public void OpenAlert(string content) => OpenAlert(string.Empty, content, string.Empty, null, 0, 0, false);
        public void OpenAlert(string content, int posX, int posY) => OpenAlert(string.Empty, content, string.Empty, null, posX, posY, false);
        public void OpenAlert(string content, System.Action action) => OpenAlert(string.Empty, content, string.Empty, action, 0, 0, false);
        public void OpenAlert(string title, string content) => OpenAlert(title, content, string.Empty, null, 0, 0, false);
        public void OpenAlert(string title, string content, System.Action action) => OpenAlert(title, content, string.Empty, action, 0, 0, false);
        public void OpenAlert(string title, string content, string btnText) => OpenAlert(title, content, btnText, null, 0, 0, false);
        public void OpenAlert(string title, string content, string btnText, System.Action action, int posX, int posY, bool autoClose) 
        {
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            string notice = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "알림", currentLocale);
            string confirm = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "확인", currentLocale);

            PopupController ctrl = Build();
            ctrl.Type = ePopupType.Alert;

            string t = string.IsNullOrEmpty(title) ? notice : title;
            ctrl.Title = t; 
            ctrl.Content = content;

            t = string.IsNullOrEmpty(btnText) ? confirm : btnText;
            switch (viewTag)
            {
                case S.WorldScene_ViewTag :
                    ctrl.AddButton(ePopupButtonType.worldNormal, t, () => action?.Invoke());
                    break;

                default :
                    ctrl.AddButton(ePopupButtonType.Normal, t, () => action?.Invoke());
                    break;
            }

            ctrl.Open(posX, posY, autoClose);
        }

        #endregion  // Alert functions


        #region Confirm functions

        public void OpenConfirm(string content, System.Action yesAction) => OpenConfirm(string.Empty, content, string.Empty, yesAction, string.Empty, null);
        public void OpenConfirm(string content, System.Action yesAction, System.Action noAction) => OpenConfirm(string.Empty, content, string.Empty, yesAction, string.Empty, noAction);
        public void OpenConfirm(string content, string yesText, System.Action yesAction, string noText, System.Action noAction) => OpenConfirm(string.Empty, content, yesText, yesAction, noText, noAction);
        public void OpenConfirm(string title, string content, System.Action yesAction) => OpenConfirm(title, content, string.Empty, yesAction, string.Empty, null);
        public void OpenConfirm(string title, string content, System.Action yesAction, System.Action noAction) => OpenConfirm(title, content, string.Empty, yesAction, string.Empty, noAction);
        public void OpenConfirm(string title, string content, string yesText, System.Action yesAction) => OpenConfirm(title, content, yesText, yesAction, string.Empty, null);
        public void OpenConfirm(string title, string content, string yesText, System.Action yesAction, string noText) => OpenConfirm(title, content, yesText, yesAction, noText, null); 
        public void OpenConfirm(string title, string content, string yesText, System.Action yesAction, string noText, System.Action noAction) 
        {
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            string notice = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "알림", currentLocale);
            string confirm = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "예", currentLocale);
            string cancel = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "아니오", currentLocale);

            PopupController ctrl = Build();
            ctrl.Type = ePopupType.Confirm;

            string t = string.IsNullOrEmpty(title) ? notice : title;
            ctrl.Title = t;
            ctrl.Content = content;

            string y = string.IsNullOrEmpty(yesText) ? confirm : yesText;
            string n = string.IsNullOrEmpty(noText) ? cancel : noText;
            switch (viewTag)
            {
                case S.WorldScene_ViewTag :
                    ctrl.AddButton(ePopupButtonType.worldNormal, y, () => yesAction?.Invoke());
                    ctrl.AddButton(ePopupButtonType.worldWarning, n, () => noAction?.Invoke());
                    break;

                default :
                    ctrl.AddButton(ePopupButtonType.Normal, y, () => yesAction?.Invoke());
                    ctrl.AddButton(ePopupButtonType.Warning, n, () => noAction?.Invoke());
                    break;
            }

            ctrl.Open();
        }

        #endregion  // Confirm functions


        #region Prompt functions

        public void OpenPrompt(string content, System.Action<string> yesAction, bool isPassword) => OpenPrompt(string.Empty, content, string.Empty, yesAction, string.Empty, null, isPassword);
        public void OpenPrompt(string title, string content, System.Action<string> yesAction, bool isPassword) => OpenPrompt(title, content, string.Empty, yesAction, string.Empty, null, isPassword);
        public void OpenPrompt(string title, string content, string yesText, System.Action<string> yesAction, string noText, System.Action noAction, bool isPassword=false) 
        {
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            string notice = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "알림", currentLocale);
            string confirm = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "예", currentLocale);
            string cancel = LocalizationSettings.StringDatabase.GetLocalizedString("Word", "아니오", currentLocale);

            PopupController ctrl = Build();
            ctrl.Type = ePopupType.Prompt;

            string t = string.IsNullOrEmpty(title) ? notice : title;
            ctrl.Title = t;
            ctrl.Content = content;
            ctrl.Password = isPassword;

            string y = string.IsNullOrEmpty(yesText) ? confirm : yesText;
            string n = string.IsNullOrEmpty(noText) ? cancel : noText;
            switch (viewTag)
            {
                case S.WorldScene_ViewTag :
                    ctrl.AddButtonWithPrompt(ePopupButtonType.worldNormal, y, (value) => yesAction?.Invoke(value));
                    ctrl.AddButton(ePopupButtonType.worldWarning, n, () => noAction?.Invoke());
                    break;

                default :
                    ctrl.AddButtonWithPrompt(ePopupButtonType.Normal, y, (value) => yesAction?.Invoke(value));
                    ctrl.AddButton(ePopupButtonType.Warning, n, () => noAction?.Invoke());
                    break;
            }

            ctrl.Open();
        }

        #endregion  // Prompt functions


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

    #endregion  // public functions
    }
}