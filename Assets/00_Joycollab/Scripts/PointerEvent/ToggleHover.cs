/// <summary>
/// Toggle 에 hover 기능을 추가하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 03. 23
/// @version        : 0.2
/// @update         :
///     v0.1 (2022. 05. 12) : 최초 생성.
/// 	v0.2 (2023. 03. 23) : TMP 대신 Legacy Text 를 사용하도록 수정.
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

        [Header("_ text")]
        [SerializeField] private Text[] _arrTxt;

        [Header("_ swap text color")]
        [SerializeField] private Color _colorTextOn;
        [SerializeField] private Color _colorTextOff;
        [SerializeField] private Color _colorTextDisabled;

        [Header("_ background")]
        [SerializeField] private Image _background;
        [SerializeField] private Color _colorBackOn;
        [SerializeField] private Color _colorBackOff;


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
                    {
                        t.color = _colorTextDisabled;
                    }
                }

                if (_background != null)
                    _background.color = _colorBackOff;
            }
            else 
            {
                if (_arrTxt.Length != 0) 
                {
                    foreach (Text t in _arrTxt) 
                    {
                        t.color = isOn ? _colorTextOn : _colorTextOff;
                    }
                }

                if (_background != null)
                    _background.color = isOn ? _colorBackOn : _colorBackOff;
            }
        }
    #endregion  // swap function
    }
}