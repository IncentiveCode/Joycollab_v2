/// <summary>
/// 여기저기 떨어져 있는 팝업 제어 함수를 하나로 묶기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 04. 13
/// @version        : 0.4
/// @update
///     v0.1 (2023. 02. 09) : TP 에서 작업했던 내용을 가지고 와서 편집. (작업중)
///     v0.2 (2023. 03. 30) : UI 최적화 (TMP 제거)
///     v0.3 (2023. 04. 13) : position 변경을 위한 rect panel 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2
{
    public class PopupController : MonoBehaviour
    {
        [Header("Basic")]
        [SerializeField] private RectTransform _rectPanel;
        [SerializeField] private Text _txtTitle; 
        [SerializeField] private Text _txtContent;

        [Header("add prompt")]
        [SerializeField] private TMP_InputField _inputPrompt;
        [SerializeField] private Button _btnClearPrompt;
        
        [Header("add option")]
        [SerializeField] private Toggle _toggleOption;
        [SerializeField] private Text _txtOption;

        [Header("Buttons")]
        [SerializeField] private Transform _transformButtons;
        [SerializeField] private GameObject _goButton;

        // local variables - for UI
        private CanvasScaler scaler;
        private float scaleRatio;

        // local variables - for popup
        private ePopupType type;
        private float displayTime;
        private float timer;

        // local variables - input field 예외처리
        private bool keepOldTextInField;
        private string oldText;
        private string editText;


    #region Unity functions
        private void Awake() 
        {
            Init();

            // set event listener
        #if UNITY_ANDROID && !UNITY_EDITOR
            _inputPrompt.onSelect.AddListener((value) => oldText = value);
            _inputPrompt.onValueChanged.AddListener((value) => {
                _btnClearPrompt.gameObject.SetActive(! string.IsNullOrEmpty(value));

                oldText = editText;
                editText = value;
            });
            _inputPrompt.onTouchScreenKeyboardStatusChanged.AddListener((value) => {
                if (value == TouchScreenKeyboard.Status.Canceled)
                {
                    keepOldTextInField = true;
                }
            });
            _inputPrompt.onDeselect.AddListener((value) => {
                if (keepOldTextInField) 
                {
                    _inputPrompt.text = oldText;
                    keepOldTextInField = false;
                }
            });
        #else
            _inputPrompt.onValueChanged.AddListener((value) => {
                _btnClearPrompt.gameObject.SetActive(! string.IsNullOrEmpty(value));
            });
        #endif
            _btnClearPrompt.onClick.AddListener(() => _inputPrompt.text = string.Empty);
        }

        private void Update() 
        {
            timer += Time.deltaTime;

            if (displayTime > 0f && type == ePopupType.Alert) 
            {
                if (timer > displayTime) 
                {
                    Close();
                }
            }

            if (timer >= 0.5f && Input.GetKey(KeyCode.Return) && type == ePopupType.Alert) 
            {
                int childCount = transform.parent.childCount;
                Transform lastChild = transform.parent.GetChild(childCount - 1);
                if (lastChild == transform)
                {
                    Close();
                }
            }
        }
    #endregion


    #region Public functions
        public ePopupType Type
        {
            get {
                return type;
            }
            set {
                type = value;
                switch (type) 
                {
                    case ePopupType.Alert :
                    case ePopupType.Confirm :
                        _inputPrompt.gameObject.SetActive(false);
                        _toggleOption.gameObject.SetActive(false);
                        break;

                    case ePopupType.Prompt :
                        _inputPrompt.gameObject.SetActive(true);
                        _toggleOption.gameObject.SetActive(false);
                        break;

                    case ePopupType.ConfirmWithOption :
                        _inputPrompt.gameObject.SetActive(false);
                        _toggleOption.gameObject.SetActive(true);
                        break; 
                }
            }
        }

        public string Title 
        {
            get {
                return _txtTitle.text;
            }
            set { 
                // _txtTitle.text = string.IsNullOrEmpty(value) ? LocalizationManager.Localize("Alert.알림") : value;
                _txtTitle.text = string.IsNullOrEmpty(value) ? "알림" : value;
            }
        }
        
        public string Content 
        {
            get {
                return _txtContent.text;
            }
            set {
                _txtContent.text = value;
            }
        }

        public string Option 
        {
            get { 
                return _txtOption.text;
            }
            set {
                _txtOption.text = value;
            }
        }

        public void AddButton(ePopupButtonType type, string text, System.Action func=null) 
        {
            var btn = Instantiate(_goButton, Vector3.zero, Quaternion.identity);
            PopupButton popupButton = btn.GetComponent<PopupButton>();
            popupButton.Init(type, text);

            btn.GetComponent<Button>().onClick.AddListener(() => {
                if (func != null) func();
                Close();
            });
            btn.transform.SetParent(_transformButtons, false);
        }

        public void AddButtonWithPrompt(ePopupButtonType type, string text, System.Action<string> func=null) 
        {
            var btn = Instantiate(_goButton, Vector3.zero, Quaternion.identity);
            PopupButton popupButton = btn.GetComponent<PopupButton>();
            popupButton.Init(type, text);

            btn.GetComponent<Button>().onClick.AddListener(() => {
                if (func != null) func(_inputPrompt.text);
                Close();
            });
            btn.transform.SetParent(_transformButtons, false);
        }

        public void AddButtonWithOption(ePopupButtonType type, string text, System.Action<bool> func=null) 
        {
            var btn = Instantiate(_goButton, Vector3.zero, Quaternion.identity);
            PopupButton popupButton = btn.GetComponent<PopupButton>();
            popupButton.Init(type, text);

            btn.GetComponent<Button>().onClick.AddListener(() => {
                if (func != null) func(_toggleOption.isOn);
                Close();
            });
            btn.transform.SetParent(_transformButtons, false);
        }

        public void Open(bool autoClose) => Open(0f, 0f, autoClose);

        public void Open(float x=0f, float y=0f, bool autoClose=false) 
        {
            if (type == ePopupType.Alert)
            {
                displayTime = autoClose ? 3f : 0f;
                timer = 0f;
            }

            Canvas.ForceUpdateCanvases();

        #if UNITY_ANDROID || UNITY_IOS
            // Mobile 은 항상 중간에 출력
        #else
            scaler = transform.parent.GetComponent<CanvasScaler>();
            scaleRatio = Util.CalculateScalerRatio(scaler);

            Vector2 temp = Vector2.zero;
            float maxX = (float) (Screen.width / scaleRatio);
            float maxY = (float) (Screen.height / scaleRatio);
            float panelWidth = _rectPanel.sizeDelta.x;
            float panelHeight = _rectPanel.sizeDelta.y;

            // 센터에 출력
            if (x == 0f && y == 0f) 
            {
                temp.x = (maxX / 2) - (panelWidth / 2);
                temp.y = (maxY / 2) - (panelHeight / 2);
            }
            // 지정 위치에 출력
            else 
            {
                temp.x = Mathf.Clamp(x, 0, maxX - panelWidth);
                temp.y = Mathf.Clamp(y, 0, maxY - panelHeight);
            }
            _rectPanel.anchoredPosition = temp;
        #endif
        }

        public void RequestClose() => Close();
    #endregion


    #region Private functions - on & off
        private void Init() 
        {
        #if UNITY_ANDROID || UNITY_IOS
            // mobile 에서는 따로 뭔가를 하지 않음.
        #else
            _rectPanel.anchoredPosition = new Vector2(-3000f, -3000f);
        #endif
        }

        private void Close() => Destroy(gameObject);

        private void Clear()
        {
            // button clear
            Transform children = _transformButtons.GetComponentInChildren<Transform>();
            foreach (Transform child in children) 
            {
                if (child.name.Equals(_transformButtons.name)) continue;
                Destroy(child.gameObject);
            }

            // contents clear
            _txtTitle.text = _txtContent.text = _txtOption.text = string.Empty;
            _inputPrompt.text = string.Empty;
            _inputPrompt.gameObject.SetActive(false);
        }

        private void Display(string title, string content, string option, float x, float y) 
        {
            _txtTitle.text = title;
            _txtContent.text = content;
            _txtOption.text = option;
            _toggleOption.gameObject.SetActive(! string.IsNullOrEmpty(option));

            Canvas.ForceUpdateCanvases();

        #if UNITY_ANDROID || UNITY_IOS
            // Mobile 은 항상 중간에 출력
        #else
            scaler = transform.parent.GetComponent<CanvasScaler>();
            scaleRatio = Util.CalculateScalerRatio(scaler);

            Vector2 temp = Vector2.zero;
            float maxX = (float) (Screen.width / scaleRatio);
            float maxY = (float) (Screen.height / scaleRatio);
            float panelWidth = _rectPanel.sizeDelta.x;
            float panelHeight = _rectPanel.sizeDelta.y;

            // 센터에 출력
            if (x == 0f && y == 0f) 
            {
                temp.x = (maxX / 2) - (panelWidth / 2);
                temp.y = (maxY / 2) - (panelHeight / 2);
            }
            // 지정 위치에 출력
            else 
            {
                temp.x = Mathf.Clamp(x, 0, maxX - panelWidth);
                temp.y = Mathf.Clamp(y, 0, maxY - panelHeight);
            }
            _rectPanel.anchoredPosition = temp;
        #endif
        }
    #endregion  // on & off
    }
}