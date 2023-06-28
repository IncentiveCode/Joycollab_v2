/// <summary>
/// [mobile]
/// 로그인 성공 후 대기 화면을 관리하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 06. 12
/// @version        : 0.5
/// @update         
///     v0.1 : 새 기획과 새 디자인 적용.
///     v0.2 (2022. 05. 25) : GetLobbyInfo() 추가.
///     v0.3 (2023. 03. 22) : FixedView 실험, UniTask 적용, UI Canvas 최적화.
///     v0.4 (2023. 03. 31) : Alarm, workspace info 관련해서도 Repository 에 저장하도록 로직 추가.
///     v0.5 (2023. 06. 12) : Legacy Text 대신 TMP Text 사용하도록 수정.
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
            ViewManager.singleton.Push(S.MobileScene_MySeat);

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
                GetMyInfo(),
                GetOfficeInfo(),
                GetAlarmList()
            );

            // text 출력
            Locale currentLocale = LocalizationSettings.SelectedLocale;
            string greetings = LocalizationSettings.StringDatabase.GetLocalizedString("Sentences", "환영인사 m", currentLocale);
            _txtGreetings.text = string.Format(greetings, R.singleton.myName);

            // TODO. 추후 office view 작업할 때 다시 적용할 것.
            // MobileManager.singleton.SetInfo();

            // TODO. FCM 관련된 항목도 Bridge app 에서 처리할 것.
            // init xmpp manager
            // XmppManager.Instance.Init();
            // XmppManager.Instance.XmppLogin($"jc-user-{R.singleton.memberSeq}", R.singleton.myXmppPw);

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
        private async UniTask<string> GetMyInfo() 
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
        private async UniTask<string> GetOfficeInfo() 
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
        private async UniTask<string> GetAlarmList() 
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

    #endregion  // Get information
    }
}