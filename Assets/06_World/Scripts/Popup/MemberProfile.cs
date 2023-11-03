/// <summary>
/// 사용자 정보 팝업
/// @author         : HJ Lee
/// @last update    : 2023. 11. 02.
/// @version        : 0.2
/// @update
///     v0.1 (2023. 10. 30) : v1 에서 만들었던 PopupUserInfo 를 수정 후 적용.
///     v0.2 (2023. 11. 02) : UI 작업 후 기능 수정 및 추가.
/// </summary>

using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class MemberProfile : WindowView
    {
        private const string TAG = "MemberProfile";

        [Header("module")] 
        [SerializeField] private ContactModule _contactModule;

        [Header("profile")]
        [SerializeField] private RawImage _imgProfile;
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private TMP_Text _txtInfo;
        [SerializeField] private TMP_Text _txtActionIndex;

        [Header("state")]
        [SerializeField] private Image _imgState;
        [SerializeField] private LocalizeStringEvent _txtState;

        [Header("button")]
        [SerializeField] private Button _btnEdit;
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnMeeting;
        [SerializeField] private Button _btnCall;
        [SerializeField] private Button _btnChat;

        // tools
        private ImageLoader loader; 
        private StringBuilder builder;

        // local variables
        private int targetMemberSeq;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();
        }

    #endregion  // Unity functions


    #region WindowView functions

        protected override void Init() 
        {
            base.Init();


            // set button listener
            _btnEdit.onClick.AddListener(() => {
                WindowManager.singleton.Push(S.WorldScene_MyProfile);
                Hide();
            });
            _btnClose.onClick.AddListener(Hide);
            _btnMeeting.onClick.AddListener(() => {
                List<int> meetingTarget = new List<int> { targetMemberSeq };
                SystemManager.singleton.MeetingOnTheSpot(meetingTarget).Forget();
            });
            _btnCall.onClick.AddListener(() => {
                List<int> callTarget = new List<int> {
                    R.singleton.memberSeq,
                    targetMemberSeq
                };
                SystemManager.singleton.CallOnTheSpot(callTarget).Forget();
            });
            _btnChat.onClick.AddListener(() => {
                string chatLink = string.Format(URL.CHATVIEW_LINK, R.singleton.memberSeq, targetMemberSeq, R.singleton.Region);
                JsLib.OpenChat(chatLink, targetMemberSeq);
            });


            // init local variables
            loader = _imgProfile.GetComponent<ImageLoader>();
            builder = new StringBuilder();
            builder.Clear();
        }

        public async override UniTaskVoid Show(int memberSeq, Vector2 pos) 
        {
            base.Show(memberSeq, pos).Forget();
            viewRect.anchoredPosition = WindowManager.singleton.CalculatePosition(pos, viewRect.sizeDelta);
            await Refresh(memberSeq);
            base.Appearing();
        }

        public override void Hide() 
        {
            base.Hide();
        }

    #endregion  // WindowView functions


    #region Event handling

        private async UniTask<int> Refresh(int memberSeq) 
        {
            PsResponse<WorkspaceMemberInfo> memberInfo = await _contactModule.GetMemberInfo(memberSeq);
            if (! string.IsNullOrEmpty(memberInfo.message)) 
            {
                Debug.Log($"{TAG} | 정보 출력 오류 : {memberInfo.message}");
                PopupBuilder.singleton.OpenAlert(memberInfo.message);
                return -1;
            }

            targetMemberSeq = memberSeq;
            DisplayInfo(memberInfo.data);
            
            return 0;
        }

        private void DisplayInfo(WorkspaceMemberInfo info) 
        {
            // profile image
            string photoPath = $"{URL.SERVER_PATH}{info.photo}";
            loader.LoadProfile(photoPath, info.seq).Forget();

            // name, member info
            _txtName.text = info.nickNm;
            builder.Clear();
            builder.Append($"{info.compName} / {info.jobGrade} \n");
            builder.Append($"{info.user.id} \n");
            // TODO. 공개 여부 체크.
            builder.Append(info.user.tel);
            _txtInfo.text = builder.ToString();

            // member action index
            _txtActionIndex.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                "Alert", "기능 준비 안내", R.singleton.CurrentLocale
            );

            // state
            _imgState.sprite = SystemManager.singleton.GetStateIcon(info.status.id);
            _txtState.StringReference.SetReference("Word", $"상태.{info.status.id}");

            // button
            bool isMine = R.singleton.memberSeq == info.seq;
            _btnEdit.gameObject.SetActive(isMine);
            _btnMeeting.interactable = !isMine;
            _btnCall.interactable = !isMine;
            _btnChat.interactable = !isMine;
        }

    #endregion  // Event handling
    }
}