/// <summary>
/// 사용자 정보 팝업
/// @author         : HJ Lee
/// @last update    : 2023. 11. 15.
/// @version        : 0.3
/// @update
///     v0.1 (2023. 10. 30) : v1 에서 만들었던 PopupUserInfo 를 수정 후 적용.
///     v0.2 (2023. 11. 02) : UI 작업 후 기능 수정 및 추가.
///     v0.3 (2023. 11. 15) : TAG 와 hiddenTel 기능 추가.
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

        [Header("tag")] 
        [SerializeField] private TagParse parser;

        [Header("button")]
        [SerializeField] private Button _btnEdit;
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnMeeting;
        [SerializeField] private Button _btnCall;
        [SerializeField] private Button _btnChat;

        [Header("toggle")]
        [SerializeField] private Toggle _toggleBookmark;

        // tools
        private ImageLoader loader; 
        private StringBuilder builder;

        // local variables
        private RectTransform _rect;
        private int _targetMemberSeq;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            _rect = transform.GetComponent<RectTransform>();
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
                List<int> meetingTarget = new List<int> { _targetMemberSeq };
                SystemManager.singleton.MeetingOnTheSpot(meetingTarget).Forget();
            });
            _btnCall.onClick.AddListener(() => {
                List<int> callTarget = new List<int> {
                    R.singleton.memberSeq,
                    _targetMemberSeq
                };
                SystemManager.singleton.CallOnTheSpot(callTarget).Forget();
            });
            _btnChat.onClick.AddListener(() => {
                string chatLink = string.Format(URL.CHATVIEW_LINK, R.singleton.memberSeq, _targetMemberSeq, R.singleton.Region);
                JsLib.OpenChat(chatLink, _targetMemberSeq);
            });


            // set toggle listener
            _toggleBookmark.onValueChanged.AddListener((isOn) => {
                // TODO. bookmark 기능 정리
                if (isOn) 
                {

                }
                else 
                {

                }
            });


            // init local variables
            loader = _imgProfile.GetComponent<ImageLoader>();
            builder = new StringBuilder();
            builder.Clear();
        }

        public override async UniTaskVoid Show(int memberSeq, Vector2 pos) 
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

            _targetMemberSeq = memberSeq;
            DisplayInfo(memberInfo.data);
            
            return 0;
        }

        private void DisplayInfo(WorkspaceMemberInfo info) 
        {
            bool isMine = (info.seq == R.singleton.memberSeq);

            // profile image
            string photoPath = $"{URL.SERVER_PATH}{info.photo}";
            loader.LoadProfile(photoPath, info.seq).Forget();

            // name, member info
            _txtName.text = info.nickNm;
            builder.Clear();
            builder.Append($"{info.compName} / {info.jobGrade} \n");
            builder.Append($"{info.user.id} \n");
            if (info.hiddenTel == false || isMine)
            {
                builder.Append(info.user.tel);
            }
            _txtInfo.text = builder.ToString();

            // member action index
            if (info.memberType.Equals(S.GUEST)) 
            {
                _txtActionIndex.text = string.Empty;
            }
            else 
            {
                builder.Clear();
                builder.Append(LocalizationSettings.StringDatabase.GetLocalizedString("Word", "등급", R.singleton.CurrentLocale));
                builder.Append(" : [등급] | ");
                builder.Append(LocalizationSettings.StringDatabase.GetLocalizedString("Word", "방문.횟수", R.singleton.CurrentLocale));
                builder.Append($" : {info.loginCnt} | ");
                builder.Append(LocalizationSettings.StringDatabase.GetLocalizedString("Word", "게시글.횟수", R.singleton.CurrentLocale));
                builder.Append($" : {info.boardCnt} | ");
                builder.Append(LocalizationSettings.StringDatabase.GetLocalizedString("Word", "댓글.횟수", R.singleton.CurrentLocale));
                builder.Append($" : {info.commentCnt}");
                _txtActionIndex.text = builder.ToString();
            }

            // state
            _imgState.sprite = SystemManager.singleton.GetStateIcon(info.status.id);
            _txtState.StringReference.SetReference("Word", $"상태.{info.status.id}");
            
            // tag
            var isTagExist = string.IsNullOrEmpty(info.tag);
            parser.gameObject.SetActive(!isTagExist);
            
            var size = _rect.sizeDelta;
            _rect.sizeDelta = new Vector2(size.x, isTagExist ? 200f : 240f);
            parser.Init(info.tag);

            // button... 수정 버튼 외의 버튼들은 주석처리
            _btnEdit.gameObject.SetActive(isMine);
            /**
            _btnMeeting.interactable = !isMine;
            _btnCall.interactable = !isMine;
            _btnChat.interactable = !isMine;
             */
            _toggleBookmark.gameObject.SetActive(! isMine);
        }

    #endregion  // Event handling
    }
}