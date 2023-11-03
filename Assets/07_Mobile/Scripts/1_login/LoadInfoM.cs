/// <summary>
/// [mobile]
/// 로그인 성공 후 대기 화면을 관리하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 07. 19
/// @version        : 0.8
/// @update         
///     v0.1 : 새 기획과 새 디자인 적용.
///     v0.2 (2022. 05. 25) : GetLobbyInfo() 추가.
///     v0.3 (2023. 03. 22) : FixedView 실험, UniTask 적용, UI Canvas 최적화.
///     v0.4 (2023. 03. 31) : Alarm, workspace info 관련해서도 Repository 에 저장하도록 로직 추가.
///     v0.5 (2023. 06. 12) : Legacy Text 대신 TMP Text 사용하도록 수정.
///     v0.6 (2023. 07. 07) : Space Info list, Bookmark list 추가 
///     v0.7 (2023. 07. 18) : member state list 추가, function name 수정.
///     v0.8 (2023. 07. 19) : unread chat count 추가.
/// </summary>

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;
using TMPro;

namespace Joycollab.v2
{
    public class LoadInfoM : FixedView
    {
        private const string TAG = "LoadInfoM";

        [Header("greetings")]
        [SerializeField] private TMP_Text _txtGreetings;
        [SerializeField] private TMP_Text _txtMessage;
        [SerializeField] private TMP_Text _txtSpeaker;


    #region Unity functions

        private void Awake() 
        {
            Init();
            base.Reset();
        }

    #endregion   // Unity functinos


    #region FixedView functions

        protected override void Init() 
        {
            base.Init();
            viewID = ID.MobileScene_LoadInfo;
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh();

            base.Appearing();
            await UniTask.Delay(1000);

            ViewManager.singleton.StartOnMySeat(true);
            // ViewManager.singleton.Push(S.MobileScene_MySeat);

            // ViewManager.singleton.Push(S.MobileScene_Office);
            // string state = JsLib.GetCookie(Key.MOBILE_FIRST_PAGE);
            // ViewManager.singleton.StartOnMySeat(state.Equals("1"));
        }

    #endregion  // FixedView functions


    #region Get information

        private async UniTask Refresh() 
        {
            await UniTask.WhenAll(
                GetLobbyInfoAsync(), 
                GetMyInfoAsync(),
                GetOfficeInfoAsync(),
                GetAlarmListAsync(),
                // GetSpaceListAsync(),
                GetBookmarkListAsync(),
                // GetStateListAsync(),
                GetUnreadCountAsync()
            );

            // text 출력
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            string greetings = LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "환영인사 m", currentLocale);
            _txtGreetings.text = string.Format(greetings, R.singleton.myName);

            // TODO. 추후 office view 작업할 때 다시 적용할 것.
            // MobileManager.singleton.SetInfo();

            // TODO. FCM 관련된 항목도 Bridge app 에서 처리할 것.

            // init xmpp manager
            SystemManager.singleton.XMPP.Init();
            SystemManager.singleton.XMPP.XmppLogin(R.singleton.myXmppId, R.singleton.myXmppPw);

            // TODO. Ping Manager 도 추가할 것.
            // init ping 
            // if (PingManager.Instance.isRun) PingManager.Instance.StopInterval();
            // PingManager.Instance.SetInterval(1);
        }


        // 1. get lobby info
        private async UniTask<string> GetLobbyInfoAsync() 
        {
            int workspaceSeq = R.singleton.workspaceSeq;
            string url = string.Format(URL.WORKSPACE_LOBBY_INFO, workspaceSeq);
            string token = R.singleton.token;
            PsResponse<LobbyInfo> res = await NetworkTask.RequestAsync<LobbyInfo>(url, eMethodType.GET, string.Empty, token);

            if (string.IsNullOrEmpty(res.message))
            {
                R.singleton.CurrentLobbyInfo = res.data;
            }
            else
            {
                Debug.Log($"{TAG} | GetLobbyInfoAsync(), error : {res.message}");
            }

            return res.message;
        }

        // 2. get my info
        private async UniTask<string> GetMyInfoAsync() 
        {
            int memberSeq = R.singleton.memberSeq;
            string url = string.Format(URL.MEMBER_INFO, memberSeq);
            string token = R.singleton.token;
            PsResponse<ResMemberInfo> res = await NetworkTask.RequestAsync<ResMemberInfo>(url, eMethodType.GET, string.Empty, token);

            if (string.IsNullOrEmpty(res.message))
            {
                R.singleton.MemberInfo = res.data;

                Debug.Log("LoadInfoM | current language : "+ res.data.lan.id);
                if (res.data.lan.id.Equals(S.REGION_KOREAN))
                    R.singleton.ChangeLocale(0);
                else
                    R.singleton.ChangeLocale(1);
            }
            else 
            {
                Debug.Log($"{TAG} | GetMyInfo(), error : {res.message}");
            }

            return res.message;
        }

        // 3. get office info
        private async UniTask<string> GetOfficeInfoAsync() 
        {
            int workspaceSeq = R.singleton.workspaceSeq;
            string url = string.Format(URL.WORKSPACE_INFO, workspaceSeq);
            string token = R.singleton.token;
            PsResponse<SimpleWorkspace> res = await NetworkTask.RequestAsync<SimpleWorkspace>(url, eMethodType.GET, string.Empty, token);

            if (string.IsNullOrEmpty(res.message))
            {
                R.singleton.SimpleWorkspaceInfo = res.data;
                JsLib.SetCookie(Key.WORKSPACE_NAME, res.data.nm);
            }
            else 
            {
                Debug.Log($"{TAG} | GetOfficeInfo(), error : {res.message}");
            }

            return res.message;
        }

        // 4. get alarm count
        private async UniTask<string> GetAlarmListAsync() 
        {
            int memberSeq = R.singleton.memberSeq;
            string token = R.singleton.token;

            string url = string.Format(URL.ALARM_LIST, memberSeq);
            PsResponse<ResAlarmList> res = await NetworkTask.RequestAsync<ResAlarmList>(url, eMethodType.GET, string.Empty, token);

            int unreadCnt = 0;
            if (string.IsNullOrEmpty(res.message)) 
            {
                foreach (ResAlarmInfo info in res.data.list) 
                {
                    R.singleton.AddAlarmInfo(info);
                    if (info.read == false) unreadCnt++;
                }

                Debug.Log($"{TAG} | unread alarm count : {unreadCnt}");
                R.singleton.UnreadAlarmCount = unreadCnt;
            }
            else 
            {
                Debug.Log($"{TAG} | GetAlarmList() : {res.message}");
                return res.message;
            }

            return string.Empty;
        }

        // 5. get space info
        private async UniTask<string> GetSpaceListAsync()
        {
	        int workspaceSeq = R.singleton.workspaceSeq;
	        string token = R.singleton.token;

            string url = string.Format(URL.SPACE_LIST, workspaceSeq);	     
            PsResponse<ResSpaceList> res = await NetworkTask.RequestAsync<ResSpaceList>(url, eMethodType.GET, string.Empty, token);

            if (string.IsNullOrEmpty(res.message))
            { 
	            foreach (ResSpaceInfo info in res.data.list)
                { 
                    Debug.Log($"{TAG} | GetSpaceList(), seq : {info.seq}, name : {info.nm} // top space seq : {info.topSpace.seq}, top space name : {info.topSpace.nm}");
                    R.singleton.AddSpace(info.seq, info);
		        } 
	        }
            else
            { 
                Debug.Log($"{TAG} | GetSpaceList() : {res.message}");
                return res.message;
	        }

            return string.Empty;
	    }

        // 6. get bookmark data
        private async UniTask<string> GetBookmarkListAsync() 
        {
            int memberSeq = R.singleton.memberSeq;
            string token = R.singleton.token;

            string url = string.Format(URL.GET_BOOKMARKS, memberSeq);
            PsResponse<ResBookmarkList> res = await NetworkTask.RequestAsync<ResBookmarkList>(url, eMethodType.GET, string.Empty, token);

            if (string.IsNullOrEmpty(res.message)) 
            {
                Bookmark t;
                foreach (var info in res.data.list) 
                {
                    t = new Bookmark(
                        info.noti != null ? eBookmarkType.Notice : eBookmarkType.Board,
                        info.seq,
                        info.noti != null ? info.noti.seq : info.board.seq
                    );

                    R.singleton.AddBookmark(t);
                }
            }
            else 
            {
                Debug.Log($"{TAG} | GetBookmarkList() : {res.message}");
                return res.message;
            }

            return string.Empty;
        }

        // 7. get member states
        /**
        private async UniTask<string> GetStateListAsync() 
        {
            string token = R.singleton.token;

            string url = string.Format(URL.GET_CODE, S.TC_MEMBER_STATUS);
            PsResponse<TpsList> res = await NetworkTask.RequestAsync<TpsList>(url, eMethodType.GET, string.Empty, token);
            if (string.IsNullOrEmpty(res.message)) 
            {
                R.singleton.ClearMemberState();
                foreach (var info in res.data.list)
                {
                    R.singleton.AddMemberState(info);
                }
            }
            else 
            {
                Debug.LogError($"{TAG} | GetStateListAsync(), error : {res.message}");
                return res.message;
            }

            return string.Empty;
        }
         */

        // 8. get unread chat count
        private async UniTask<string> GetUnreadCountAsync() 
        {
            string token = R.singleton.token;
            int memberSeq = R.singleton.memberSeq;

            string url = string.Format(URL.UNREAD_CHAT_COUNT, memberSeq);
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.GET, string.Empty, token);
            if (string.IsNullOrEmpty(res.message)) 
            {
                int temp = -1;
                int.TryParse(res.stringData, out temp);
                R.singleton.UnreadChatCount = temp;
            }
            else 
            {
                Debug.LogError($"{TAG} | GetUnreadCountAsync(), error : {res.message}");
                return res.message;
            }

            return string.Empty;
        }

        // 6. get member data
        /**
        private async UniTask<string> GetMemberPart() 
        {
            string token = R.singleton.token;
            int workspaceSeq = R.singleton.workspaceSeq;

            string url = string.Format(URL.WORKSPACE_MEMBER_LIST, workspaceSeq);
            PsResponse<WorkspaceMemberList> res = await NetworkTask.RequestAsync<WorkspaceMemberList>(url, eMethodType.GET, string.Empty, token);

            if (string.IsNullOrEmpty(res.message)) 
            {
                foreach (var item in res.data.list) 
                {
                    if (item.
                }
            }
            else 
            {
                Debug.Log($"{TAG} | GetMemberList() : {res.message}");
                return res.message;
            }

            return string.Empty;
        }
         */

    #endregion  // Get information
    }
}