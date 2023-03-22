/// <summary>
/// [mobile]
/// 로그인 성공 후 대기 화면을 관리하는 클래스.
/// @author         : HJ Lee
/// @last update    : 2023. 03. 22
/// @version        : 0.3
/// @update         
///     v0.1 : 새 기획과 새 디자인 적용.
///     v0.2 (2022. 05. 25) : GetLobbyInfo() 추가.
///     v0.3 (2023. 03. 22) : FixedView 실험, UniTask 적용, UI Canvas 최적화.
/// </summary>

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class LoadInfoM : FixedView
    {
        [Header("greetings")]
        [SerializeField] private Text _txtGreetings;
        [SerializeField] private Text _txtMessage;
        [SerializeField] private Text _txtSpeaker;


    #region Unity functions
        private void Awake() 
        {
            Init();
            Reset();
        }
    #endregion   // Unity functinos


    #region FixedView functions
        protected override void Init() 
        {
            base.Init();
        }

        protected override void Reset() 
        {
            base.Reset();
        }

        public async override UniTaskVoid Show() 
        {
            base.Show().Forget();

            await Refresh();

            base.Appearing().Forget();

            await UniTask.Delay(1000);

            MobileManager.singleton.Push(S.MobileScene_Office);

            string state = JsLib.GetCookie(Key.MOBILE_FIRST_PAGE);
            MobileManager.singleton.StartOnMySeat(state.Equals("1"));
        }
    #endregion  // FixedView functions


    #region Get information
        private async UniTask Refresh() 
        {
            await UniTask.WhenAll(
                GetLobbyInfoAsync(), 
                GetMyInfo(),
                GetOfficeInfo(),
                GetAlarmCount()
            );
        }


        // 1. get lobby info
        private async UniTask<string> GetLobbyInfoAsync() 
        {
            int workspaceSeq = Storage.workspaceSeq;
            string url = string.Format(URL.WORKSPACE_LOBBY_INFO, workspaceSeq);
            string token = Storage.token;
            PsResponse<LobbyInfo> res = await NetworkTask.RequestAsync<LobbyInfo>(url, MethodType.GET, string.Empty, token);

            // TODO. 로비 정보 저장. (in Storage)
            if (! string.IsNullOrEmpty(res.message)) Debug.Log("GetLobbyInfoAsync() - "+ res.message);
            return res.message;
        }

        // 2. get my info
        private async UniTask<string> GetMyInfo() 
        {
            int memberSeq = Storage.memberSeq;
            string url = string.Format(URL.MEMBER_INFO, memberSeq);
            string token = Storage.token;
            PsResponse<ResMemberInfo> res = await NetworkTask.RequestAsync<ResMemberInfo>(url, MethodType.GET, string.Empty, token);

            // TODO. 내 정보 저장. (in Storage)
            if (! string.IsNullOrEmpty(res.message)) 
                Debug.Log("GetMyInfo() - "+ res.message);
            else 
                _txtGreetings.text = $"환영합니다, {res.data.nickNm} 님";

            return res.message;
        }

        // 3. get office info
        private async UniTask<string> GetOfficeInfo() 
        {
            int workspaceSeq = Storage.workspaceSeq;
            string url = string.Format(URL.WORKSPACE_INFO, workspaceSeq);
            string token = Storage.token;
            PsResponse<SimpleWorkspace> res = await NetworkTask.RequestAsync<SimpleWorkspace>(url, MethodType.GET, string.Empty, token);

            // TODO. 워크스페이스 정보 저장. (in Storage)
            if (! string.IsNullOrEmpty(res.message)) Debug.Log("GetOfficeInfo() - "+ res.message);
            return res.message;
        }

        // 4. get alarm count
        private async UniTask<string> GetAlarmCount() 
        {
            int memberSeq = Storage.memberSeq;
            string url = string.Format(URL.ALARM_COUNT, memberSeq);
            string token = Storage.token;
            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, MethodType.GET, string.Empty, token);

            // TODO. 알림 카운트 저장. (in Storage)
            if (! string.IsNullOrEmpty(res.message)) Debug.Log("GetAlarmCount() - "+ res.message);
            return res.message;
        }
    #endregion  // Get information
    }
}