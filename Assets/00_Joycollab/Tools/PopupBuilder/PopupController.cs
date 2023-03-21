/// <summary>
/// [PC Web]
/// 여기저기 떨어져 있는 팝업 제어 함수를 하나로 묶기 위한 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 02. 09
/// @version        : 0.1
/// @update
///     - v0.1 : TP 에서 작업했던 내용을 가지고 와서 편집. (작업중)
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;
// using Assets.SimpleLocalization;

namespace Joycollab.v2
{
    public class PopupController : MonoBehaviour
    {
        [Header("Basic")]
        [SerializeField] private TMP_Text _txtTitle; 
        [SerializeField] private TMP_Text _txtContent;

        [Header("add prompt")]
        [SerializeField] private TMP_InputField _inputPrompt;
        [SerializeField] private Button _btnClearPrompt;
        
        [Header("add option")]
        [SerializeField] private Toggle _toggleOption;
        [SerializeField] private TMP_Text _txtOption;

        [Header("Buttons")]
        [SerializeField] private Transform _transformButtons;
        [SerializeField] private GameObject _goButton;

        // local variables - for UI
        private CanvasScaler scaler;
        private float scaleRatio;
        private RectTransform rect;

        // local variables - for popup
        private PopupType type;
        private float displayTime;
        private float timer;


    #region Unity functions
        private void Awake() 
        {
            rect = transform.GetChild(0).GetComponent<RectTransform>();

            // set event listener
            _inputPrompt.onValueChanged.AddListener((value) => {
                _btnClearPrompt.gameObject.SetActive(! string.IsNullOrWhiteSpace(value));
            });
            _btnClearPrompt.onClick.AddListener(() => _inputPrompt.text = string.Empty);
        }

        private void Update() 
        {
            timer += Time.deltaTime;

            if (displayTime > 0f && type == PopupType.Alert) 
            {
                if (timer > displayTime) 
                {
                    Close();
                }
            }

            if (timer >= 0.5f && Input.GetKey(KeyCode.Return) && type == PopupType.Alert) 
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

    #endregion


    #region Private functions - popup setting
    #endregion // popup setting


    #region Private functions - on & off
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
            scaler = transform.parent.GetComponent<CanvasScaler>();
            scaleRatio = Util.CalculateScalerRatio(scaler);

            _txtTitle.text = title;
            _txtContent.text = content;
            _txtOption.text = option;
            _toggleOption.gameObject.SetActive(! string.IsNullOrEmpty(option));

            Canvas.ForceUpdateCanvases();

            Vector2 temp = Vector2.zero;
            float maxX = (float) (Screen.width / scaleRatio);
            float maxY = (float) (Screen.height / scaleRatio);
            float panelWidth = rect.sizeDelta.x;
            float panelHeight = rect.sizeDelta.y;

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
            rect.anchoredPosition = temp;
        }
    #endregion  // on & off
    }
}