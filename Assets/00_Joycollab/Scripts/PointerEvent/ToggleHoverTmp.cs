/// <summary>
/// Toggle 에 hover 기능을 추가하는 클래스. (Tmp 용으로 추가)
/// @author         : HJ Lee
/// @last update    : 2023. 10. 05
/// @version        : 0.5
/// @update         :
///     v0.1 (2023. 10. 05) : ToggleHover 를 Tmp 용으로 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2 
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleHoverTmp : MonoBehaviour
    {
        private Toggle toggle;
        private bool currentState;
        private bool checkState; 

        [Header("text list")]
        [SerializeField] private TMP_Text[] _arrTxt;

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
                    foreach (var t in _arrTxt) 
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
                    foreach (var t in _arrTxt) 
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