/// <summary>
/// [world]
/// 모임방 리스트 항목 Script
/// @author         : HJ Lee
/// @last update    : 2023. 12. 22
/// @version        : 0.2
/// @update
///     v0.1 (2023. 09. 13) : 최초 생성
///     v0.2 (2023. 12. 22) : 특정 모임방으로 이동하는 기능 추가.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using Gpm.Ui;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class RoomItem : InfiniteScrollItem
    {
        private const string TAG = "RoomItem";

        [Header("image")]
        [SerializeField] private RawImage _imgLogo;
        
		[Header("button")]
        [SerializeField] private Button _btnItem;
		[SerializeField] private Button _btnEnter;
		[SerializeField] private Button _btnJoin;

		[Header("text")]
		[SerializeField] private TMP_Text _txtTitle;
		[SerializeField] private TMP_Text _txtDesc;
		[SerializeField] private TMP_Text _txtPublicOpt;
		[SerializeField] private TMP_Text _txtOwner;

        // local variables
        private ClasData data;
        private int seq;
        private ImageLoader loader;


    #region Unity functions

        private void Awake() 
        {
            // set button listener
            _btnItem.onClick.AddListener(OnSelect);
            _btnEnter.onClick.AddListener(() => OnEnterAsync().Forget());
            _btnJoin.onClick.AddListener(OnJoin);


            // init image loader
            loader = _imgLogo.GetComponent<ImageLoader>();
        }

    #endregion  // Unity functions


    #region GPM functions

        public override void UpdateData(InfiniteScrollData scrollData) 
        {
            base.UpdateData(scrollData);

            data = (ClasData) scrollData;
            this.seq = data.info.seq;

            // 이미지 로드
            string logoPath = string.Format("{0}{1}", URL.SERVER_PATH, data.info.logo);
            if (string.IsNullOrEmpty(data.info.logo))
            {
                loader.SetDefault();
            }
            else
            {
                loader.LoadImage(logoPath).Forget();
            }

            // 문구 정리
            _txtTitle.text = data.info.nm;
            _txtDesc.text = data.info.clas.bigo;
            _txtPublicOpt.text = LocalizationSettings.StringDatabase.GetLocalizedString("Word", data.info.clas.openType, R.singleton.CurrentLocale);
            _txtOwner.text = data.info.ceoNm;

            // 버튼 정리
            // TODO. 내 가입 여부에 따라 '입장' 버튼과 '가입' 버튼을 출력. 지금은 무조건 입장 버튼만 출력함.
            _btnEnter.gameObject.SetActive(true); 
            _btnJoin.gameObject.SetActive(false);
        }

    #endregion  // GPM functions


    #region event handling

        private async UniTaskVoid OnEnterAsync() 
        {
            Debug.Log($"{TAG} | OnEnter(), TODO.진입 이벤트 추가 예정.");

            // TODO. 정보 확보
            PsResponse<ResWorkspaceList> res = await NetworkTask.RequestAsync<ResWorkspaceList>(URL.WORKSPACE_LIST, eMethodType.GET, string.Empty, R.singleton.token);
            if (! string.IsNullOrEmpty(res.message))
            {
                PopupBuilder.singleton.OpenAlert(res.message);
                return;
            }

            int nextSpaceSeq = 0;
            int nextMemberSeq = 0;
            string nextRoomType = string.Empty;
            foreach (var space in res.data.list) 
            {
                if (space.workspace.seq == this.seq && space.workspace.workspaceType.Equals(S.GROUP)) 
                {
                    nextSpaceSeq = this.seq;
                    nextMemberSeq = space.seq;
                    nextRoomType = space.workspace.clas.themes.id;
                    break;
                }
            }

            if (nextSpaceSeq == 0 && nextMemberSeq == 0) 
            {
                PopupBuilder.singleton.OpenAlert("모임방 정보를 확인할 수 없습니다.");
                return;
            }


            // TODO. 멤버 정보 확인
            string url = string.Format(URL.MEMBER_INFO, nextMemberSeq);
            PsResponse<ResMemberInfo> res2 = await NetworkTask.RequestAsync<ResMemberInfo>(url, eMethodType.GET, string.Empty, R.singleton.token);
            if (! string.IsNullOrEmpty(res2.message)) 
            {
                PopupBuilder.singleton.OpenAlert(res2.message);
                return;
            }


            // TODO. 정보 반영
            R.singleton.MemberInfo = res2.data;
            switch (res2.data.lan.id) 
            {
                case S.REGION_KOREAN :
                    R.singleton.ChangeLocale(ID.LANGUAGE_KOREAN);
                    break;

                case S.REGION_JAPANESE :
                    R.singleton.ChangeLocale(ID.LANGUAGE_JAPANESE);
                    break;
                
                default :
                    R.singleton.ChangeLocale(ID.LANGUAGE_ENGLISH);
                    break;
            }    

            // Debug.Log($"{TAG} | current font size : {res.data.fontSize}");
            R.singleton.FontSizeOpt = res2.data.fontSize;
            R.singleton.ID = res2.data.user.id;

            WorldAvatarInfo info = new WorldAvatarInfo(res2.data);
            info.roomTypeId = nextRoomType;
            WorldPlayer.localPlayerInfo = info;


            // TODO. 이전 정보 처리
            SystemManager.singleton.BeforeMoveGroup(); 
            MultiSceneNetworkManager.singleton.StopClient();


            // TODO. 정보 저장
            JsLib.SetCookie(Key.WORKSPACE_SEQ, nextSpaceSeq.ToString());
            JsLib.SetCookie(Key.MEMBER_SEQ, nextMemberSeq.ToString());

            // Repo.Instance.workspaceSeq = nextSpaceSeq; 
            // Repo.Instance.memberSeq = nextMemberSeq; 
            R.singleton.workspaceSeq = nextSpaceSeq;
            R.singleton.memberSeq = nextMemberSeq;


            // TODO. 후속 조치
            SystemManager.singleton.AfterMoveGroup();
            SceneLoader.Load(eScenes.Square);
        }

        private void OnJoin() 
        {
            Debug.Log($"{TAG} | OnJoin(), TODO.가입 이벤트 추가 예정... room seq : {this.seq}");
        }

    #endregion  // event handling
    }
}