/// <summary>
/// Floating bar 클래스
/// @author         : HJ Lee
/// @last update    : 2023. 09. 18 
/// @version        : 0.1
/// @update
///     v0.1 (2023. 09. 18) : v1 에서 사용하던 항목 수정 후 적용. (진행 중)
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2
{
    public class FloatingBar : MonoBehaviour
    {
        private const string TAG = "FloatingBar";

        [Header("for regular user")]
        [SerializeField] private Image _imgPanel;
        [SerializeField] private Button _btnFile;
        [SerializeField] private Button _btnTodo;
        [SerializeField] private Button _btnChat;
        [SerializeField] private Image _imgChatCnt;
        [SerializeField] private TMP_Text _txtChatCnt;
        [SerializeField] private Button _btnMeeting;
        [SerializeField] private Button _btnContact;
        [SerializeField] private Button _btnKanban;
        [SerializeField] private Button _btnCalendar;
        [SerializeField] private Button _btnBoard;

        [Header("for guest")]
        [SerializeField] private Image _imgGuestPanel;
        [SerializeField] private Button _btnTutorial;
        [SerializeField] private Button _btnCallManager;
        [SerializeField] private Button _btnSendFeedback;
        [SerializeField] private Button _btnSeminar;
        [SerializeField] private Button _btnGuestExit;


    #region Unity functions

        private void Awake() 
        {
            InitForRegular();
            InitForGuest();
        }        

    #endregion  // Unity functions


    #region Initialize

        private void InitForRegular() 
        {

        }

        private void InitForGuest() 
        {

        }

    #endregion  // Initialize
    }
}