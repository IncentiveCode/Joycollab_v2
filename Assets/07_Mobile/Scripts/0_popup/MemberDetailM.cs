/// <summary>
/// [mobile]
/// 사용자 팝업 화면을 담당하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 28
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 28) : 최초 생성
/// </summary>

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class MemberDetailM : FixedView
    {
        private const string TAG = "MemberDetailM";

        [Header("module")]
        [SerializeField] private ContactModule _module;

        [Header("photo")]
        [SerializeField] private RawImage _imgPhoto;

        [Header("state")]
        [SerializeField] private Image _imgState;
        [SerializeField] private TMP_Text _txtState;
        
        [Header("text")]
        [SerializeField] private TMP_Text _txtName;
        [SerializeField] private TMP_Text _txtDesc;
        [SerializeField] private TMP_Text _txtMail;
        [SerializeField] private TMP_Text _txtMessage;

        [Header("button")]
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnFileBox;
        [SerializeField] private Button _btnTodo;
        [SerializeField] private Button _btnMeeting;
        [SerializeField] private Button _btnVoiceCall;
        [SerializeField] private Button _btnChat;

        // local variables
        private ImageLoader imageLoader;
        private int seq;
        private bool isMyInfo;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();

            // add event handling
            MobileEvents.singleton.OnBackButtonProcess += BackButtonProcess;
        }

        private void OnDestroy() 
        {
            if (MobileEvents.singleton != null) 
            {
                MobileEvents.singleton.OnBackButtonProcess -= BackButtonProcess;
            }
        }

    #endregion  // Unity functions


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();
            viewID = ID.MobileScene_MemberDetail;


            // set 'button' listener
            _btnClose.onClick.AddListener(() => ViewManager.singleton.CloseOverlay());

            _btnFileBox.onClick.AddListener(() => {
                if (isMyInfo)
                {
                    ViewManager.singleton.CloseOverlay();
                    ViewManager.singleton.Push(S.MobileScene_FileBox, "-1|/");
                }
                else 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    string text = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "타인의 파일함 접근 불가", currentLocale);
                    PopupBuilder.singleton.OpenAlert(text);
                }
            });

            _btnTodo.onClick.AddListener(() => { 
                ViewManager.singleton.CloseOverlay();
                ViewManager.singleton.Push(S.MobileScene_ToDo, seq.ToString());
            });

            _btnMeeting.onClick.AddListener(() => {
                if (isMyInfo) 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    string text = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "자신과의 화상회의 사용 불가", currentLocale);
                    PopupBuilder.singleton.OpenAlert(text);
                }
                else 
                {
                    List<int> seqs = new List<int>(1);
                    seqs.Add(seq);
                    PopupBuilder.singleton.OpenAlert("화상회의 준비 중 입니다.");
                    ViewManager.singleton.CloseOverlay();


                    // TODO. 권한 체크
                    /**
                    if (Repo.Instance.CheckAuth(Repo.Instance.MeetingRoomSeq, Strings.CREATE_MEETING))
                    {
                        memberSeqs.Add(selectMemberSeq);
                        MeetingManager.Instance.MeetingMembers(memberSeqs);
                    }
                    else
                    {
                        string title = LocalizationManager.Localize("Alert.권한");
                        string content = LocalizationManager.Localize("Meeting.회의생성권한없음");
                        MainViewManager.Instance.ShowAlert(content, title);
                    }
                     */
                }
            });

            _btnVoiceCall.onClick.AddListener(() => {
                if (isMyInfo) 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    string text = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "자신과의 음성통화 사용 불가", currentLocale);
                    PopupBuilder.singleton.OpenAlert(text);
                }
                else 
                {
                    // TODO. meeting manager 생성 후 연결
                    PopupBuilder.singleton.OpenAlert("음성 통화 준비 중 입니다.");
                    ViewManager.singleton.CloseOverlay();
                }
            });

            _btnChat.onClick.AddListener(() => {
                if (isMyInfo) 
                {
                    Locale currentLocale = LocalizationSettings.SelectedLocale;
                    string text = LocalizationSettings.StringDatabase.GetLocalizedString("Alert", "자신과의 채팅 사용 불가", currentLocale);
                    PopupBuilder.singleton.OpenAlert(text);
                }
                else 
                {
                    // TODO. meeting manager 생성 후 연결
                    PopupBuilder.singleton.OpenAlert("채팅 준비 중 입니다.");
                    ViewManager.singleton.CloseOverlay();
                }
            });


            // init local variables
            imageLoader = _imgPhoto.GetComponent<ImageLoader>();
            seq = -1;
            isMyInfo = false;
        }

        public override async UniTaskVoid Show(string opt) 
        {
            base.Show().Forget();

            int temp = -1;
            int.TryParse(opt, out temp);

            seq = temp;
            isMyInfo = (R.singleton.memberSeq == seq);

            await Refresh();
            base.Appearing();
        }

    #endregion  // FixedView functions


    #region for info

        private async UniTask<int> GetInfo() 
        {
            if (seq == -1) 
            {
                Debug.Log($"{TAG} | GetInfo(), seq 오류.");
                return 0;
            }

            PsResponse<WorkspaceMemberInfo> res = await _module.GetMemberInfo(seq);
            if (! string.IsNullOrEmpty(res.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return 0;
            }

            PsResponse<ResSpaceInfo> res2 = await _module.GetSpaceInfo(res.data.space.seq);
            string spaceNm = string.Empty;
            if (string.IsNullOrEmpty(res2.message)) 
                spaceNm = res2.data.nm;
            else 
                Debug.Log($"{TAG} | GetInfo(), get space info fail. message : {res2.message}");


            // TODO. avatar status sync.

            // set button
            /**
            if (_btnFileBox != null) _btnFileBox.interactable = isMyInfo;
            if (_btnTodo != null) _btnTodo.interactable = true;
            if (_btnMeeting != null) _btnMeeting.interactable = !isMyInfo;
            if (_btnVoiceCall != null) _btnVoiceCall.interactable = !isMyInfo;
            if (_btnChat != null) _btnChat.interactable = !isMyInfo;
             */

            // set photo
            if (string.IsNullOrEmpty(res.data.photo)) 
            {
                imageLoader.LoadProfile(string.Empty, seq).Forget();
            }
            else 
            {
                string url = $"{URL.SERVER_PATH}{res.data.photo}";
                imageLoader.LoadProfile(url, seq).Forget();
            }

            // set status
            if (_imgState != null) 
            {
                Locale currentLocale = LocalizationSettings.SelectedLocale;
                string temp = string.Format("상태.({0})", res.data.status.id);
                _txtState.text = LocalizationSettings.StringDatabase.GetLocalizedString("Word", temp, currentLocale);

                switch (res.data.status.id) 
                {
                    case S.ONLINE :             _imgState.color = C.ONLINE;             break;
                    case S.OFFLINE :            _imgState.color = C.OFFLINE;            break;
                    case S.MEETING :            _imgState.color = C.MEETING;            break;
                    case S.LINE_BUSY :          _imgState.color = C.LINE_BUSY;          break;
                    case S.BUSY :               _imgState.color = C.BUSY;               break;
                    case S.OUT_ON_BUSINESS :    _imgState.color = C.OUT_ON_BUSINESS;    break;
                    case S.OUTING :             _imgState.color = C.OUT_ON_BUSINESS;    break;
                    case S.NOT_HERE :           _imgState.color = C.NOT_HERE;           break;
                    case S.DO_NOT_DISTURB :     _imgState.color = C.DO_NOT_DISTURB;     break;
                    case S.VACATION :           _imgState.color = C.VACATION;           break;
                    case S.NOT_AVAILABLE :      _imgState.color = C.NOT_AVAILABLE;      break;
                    default :                   _imgState.color = C.ONLINE;             break;
                }
            }

            // set texts
            if (_txtName != null) _txtName.text = res.data.nickNm;
            if (_txtDesc != null) 
            {
                if (res.data.memberType.Equals(S.GUEST))
                    _txtDesc.text = "<color=blue>Guest</color>";
                else
                    _txtDesc.text = $"{spaceNm} | {res.data.jobGrade}";
            }
            if (_txtMail != null) _txtMail.text = res.data.user.id;
            if (_txtMessage != null) _txtMessage.text = res.data.description;

            return 0;
        }

    #endregion  // for info


    #region event handling

        private async UniTask<int> Refresh() 
        {
            // view control
            ViewManager.singleton.ShowNavigation(false);

            // get user info
            await GetInfo();

            return 0;
        }

        private void BackButtonProcess(string name="") 
        {
            if (! name.Equals(gameObject.name)) return; 
            if (visibleState != eVisibleState.Appeared) return;

            if (PopupBuilder.singleton.GetPopupCount() > 0)
            {
                PopupBuilder.singleton.RequestClear();
            }
            else 
            {
                BackProcess();
            }
        }

        private void BackProcess() 
        {
            ViewManager.singleton.CloseOverlay();
        }

    #endregion  // event handling
    }
}