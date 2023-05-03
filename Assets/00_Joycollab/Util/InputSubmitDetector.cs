/// <summary>
/// InputField 의 Submit 을 탐지하는 클래스 
/// @author         : HJ Lee
/// @last update    : 2023. 03. 21
/// @version        : 0.1
/// @update
///     v0.1 (2023. 03. 20) : 최초 생성
/// </summary>

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Joycollab.v2
{
    public class InputSubmitDetector : InputField
    {
        [Serializable] public class KeyboardDoneEvent : UnityEvent {} 

        [SerializeField]
        private KeyboardDoneEvent m_keyboardDone = new KeyboardDoneEvent();
        public KeyboardDoneEvent onKeyboardDone {
            get { return m_keyboardDone; }
            set { m_keyboardDone = value; }
        }

        private void Update() 
        {
            if (m_Keyboard != null && m_Keyboard.status == TouchScreenKeyboard.Status.Done && m_Keyboard.status != TouchScreenKeyboard.Status.Canceled) 
            {
                m_keyboardDone.Invoke();
            }
        }
    }
}