/// <summary>
/// Toggle 에 hover 기능을 추가하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 08. 24
/// @version        : 0.4
/// @update         :
///     v0.1 (2022. 05. 12) : 최초 생성.
/// 	v0.2 (2023. 03. 23) : TMP 대신 Legacy Text 를 사용하도록 수정.
/// 	v0.3 (2023. 08. 23) : toggle 안에 이미지가 있는 경우, 해당 이미지도 색상 변경이 되게 수정.
/// 	v0.4 (2023. 08. 24) : 이미지와 텍스트 색상이 다르게 설정해야 하는 경우도 대비할 수 있게 수정.
/// </summary>

using UnityEngine;
using UnityEngine.UI;

namespace Joycollab.v2 
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleHover : MonoBehaviour
    {
        private Toggle toggle;
        private bool currentState;
        private bool checkState; 

        [Header("text list")]
        [SerializeField] private Text[] _arrTxt;

        [Header("text color")]
        [SerializeField] private Color _colorContentOn;
        [SerializeField] private Color _colorContentOff;
        [SerializeField] private Color _colorContentDisabled;

        [Header("image list")]
        [SerializeField] private Image[] _arrImage;

        [Header("image color")]
        [SerializeField] private Color _colorImageOn;
        [SerializeField] private Color _colorImageOff;
        [SerializeField] private Color _colorImageDisabled;

        [Header("background")]
        [SerializeField] private Image _background;
        [SerializeField] private Color _colorBackgroundOn;
        [SerializeField] private Color _colorBackgroundOff;


    #region Unity functions

        private void Awake() 
        {
            toggle = gameObject.GetComponent<Toggle>();
            currentState = toggle.interactable;
            checkState = toggle.isOn;

            Set(); 
        }

        private void Update() 
        {
            if (currentState != toggle.interactable || checkState != toggle.isOn) 
            {
                Set();

                currentState = toggle.interactable;
                checkState = toggle.isOn;
            }
        }

    #endregion  // Unity function


    #region text, image swap function

        private void Set() 
        {
            bool state = toggle.interactable;
            bool isOn = toggle.isOn;

            if (! state) 
            {
                if (_arrTxt.Length != 0) 
                {
                    foreach (Text t in _arrTxt) 
                        t.color = _colorContentDisabled;
                }

                if (_arrImage.Length != 0) 
                {
                    foreach (Image img in _arrImage) 
                        img.color = _colorImageDisabled;
                }

                if (_background != null)
                    _background.color = _colorBackgroundOff;
            }
            else 
            {
                if (_arrTxt.Length != 0) 
                {
                    foreach (Text t in _arrTxt) 
                        t.color = isOn ? _colorContentOn : _colorContentOff;
                }

                if (_arrImage.Length != 0)
                {
                    foreach (Image img in _arrImage) 
                        img.color = isOn ? _colorImageOn : _colorImageOff;
                }

                if (_background != null)
                    _background.color = isOn ? _colorBackgroundOn : _colorBackgroundOff;
            }
        }

    #endregion  // swap function
    }
}