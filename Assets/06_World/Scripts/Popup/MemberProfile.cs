/// <summary>
/// 사용자 정보 팝업
/// @author         : HJ Lee
/// @last update    : 2023. 10. 30.
/// @version        : 0.1
/// @update
///     v0.1 (2023. 10. 30) : v1 에서 만들었던 PopupUserInfo 를 수정 후 적용.
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Joycollab.v2
{
    public class MemberProfile : MonoBehaviour
    {
        private const string TAG = "MemberProfile";

        [Header("common")] 
        [SerializeField] private Image _imgPanel;
        [SerializeField] private TMP_Text _txtName;

        [Header("button")]
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnEdit;
        [SerializeField] private Button _btnState;
        [SerializeField] private Button _btnFindAddress;
        [SerializeField] private Button _btnSave;

        [Header("input field")]
        [SerializeField] private TMP_InputField _inputName;
        [SerializeField] private TMP_InputField _inputOffice;
        [SerializeField] private TMP_InputField _inputGrade;
        [SerializeField] private TMP_InputField _inputPhone;

    }
}