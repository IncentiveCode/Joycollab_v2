/// <summary>
/// Login 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 08. 11
/// @version        : 0.3
/// @update
///     v0.1 (2023. 05. 09) : 최초 생성
///     v0.2 (2023. 05. 10) : LoginScene 에서 사용하던 Login 관련 기능 정리.
///     v0.3 (2023. 08. 11) : WorldScene 에서 사용하는 Login 관련 기능 정리.
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class LoginModule : MonoBehaviour
    {
        private const string TAG = "LoginModule";


    #region workspace functions

        /// <summary>
        /// 현재 cookie 에 있는 Token 유효성을 확인
        /// </summary>
        /// <returns>NetworkTask request 결과물 : PsResponse<ResCheckToken></returns>
        public async UniTask<PsResponse<ResCheckToken>> CheckTokenAsync() 
        {
            string type = JsLib.GetCookie(Key.TOKEN_TYPE);
            string token = JsLib.GetCookie(Key.ACCESS_TOKEN);
            string workspaceSeq = JsLib.GetCookie(Key.WORKSPACE_SEQ);
            string memberSeq = JsLib.GetCookie(Key.MEMBER_SEQ);


            // step 1. cookie check
            if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(workspaceSeq) || string.IsNullOrEmpty(memberSeq)) 
            {
                Debug.Log($"{TAG} | CheckTokenAsync(), without cookie value");

                R.singleton.Clear();
                Tmp.singleton.Clear();
                return null;
            }


            // step 2. call NetworkTask
            WWWForm form = new WWWForm();
            form.AddField(NetworkTask.AUTHORIZATION, NetworkTask.BASIC_TOKEN);
            form.AddField(Key.TOKEN, token);

            string url = string.Format(URL.CHECK_TOKEN, token);
            PsResponse<ResCheckToken> res = await NetworkTask.PostAsync<ResCheckToken>(url, form, NetworkTask.CONTENT_JSON, NetworkTask.BASIC_TOKEN);
            res.extra = $"{type} {token}";
            return res;
        }


        /// <summary>
        /// id, pw 를 사용하는 로그인
        /// </summary>
        /// <param name="id">사용자 id (e-mail)</param>
        /// <param name="pw">사용자 password</param>
        /// <returns>NetworkTask request 결과물 : PsResponse<ResToken></returns>
        public async UniTask<PsResponse<ResToken>> LoginAsync(string id, string pw) 
        {
            string token = string.Empty;

            WWWForm form = new WWWForm();
            form.AddField(NetworkTask.GRANT_TYPE, NetworkTask.GRANT_TYPE_PW);
            form.AddField(NetworkTask.PASSWORD, pw);
            form.AddField(NetworkTask.REFRESH_TOKEN, string.Empty);
        #if UNITY_ANDROID || UNITY_IOS
            form.AddField(NetworkTask.SCOPE, NetworkTask.SCOPE_APP);
            token = NetworkTask.BASIC_TOKEN_M;
        #else
            form.AddField(NetworkTask.SCOPE, NetworkTask.SCOPE_ADM);
            token = NetworkTask.BASIC_TOKEN;
        #endif
            form.AddField(NetworkTask.USERNAME, id);

            PsResponse<ResToken> res = await NetworkTask.PostAsync<ResToken>(URL.REQUEST_TOKEN, form, string.Empty, token);
            return res;
        }

        // TODO. GUEST LOGIN 추가

    #endregion  // workspace functions


    #region world functions

        public async UniTask<PsResponse<ResWorkspaceInfo>> WorldSignInAsync() 
        {
            // - step 1. workspace seq check
            int workspaceSeq = await GetWorldSequenceAsync();
            if (workspaceSeq == -1) 
            {
                return new PsResponse<ResWorkspaceInfo>(-1, string.Empty);
            }

            // - step 2. member seq check
            string message = await CheckExistMemberAsync(workspaceSeq);
            if (! string.IsNullOrEmpty(message)) 
            {
                // - step 3. join
                message = await JoinWorldAsync(workspaceSeq);
                if (! string.IsNullOrEmpty(message)) 
                {
                    return new PsResponse<ResWorkspaceInfo>(-1, message);
                }
            }

            PsResponse<ResWorkspaceInfo> res = await GetWorkspaceInfoAsync(workspaceSeq);
            return res;
        }

        // - step 1. 'world' 의 workspace seq check
        private async UniTask<int> GetWorldSequenceAsync() 
        {
            int result = -1;

            string url = string.Format(URL.CHECK_DOMAIN, S.WORLD);
            PsResponse<SimpleWorkspace> res = await NetworkTask.RequestAsync<SimpleWorkspace>(url, eMethodType.GET);
            if (string.IsNullOrEmpty(res.message)) 
            {
                result = res.data.seq;
            }

            return result;
        }

        // step 2. 나의 member seq check
        private async UniTask<string> CheckExistMemberAsync(int workspaceSeq) 
        {
            string url = string.Format(URL.WORKSPACE_INFO, workspaceSeq);
            string token = R.singleton.token;

            PsResponse<SimpleWorkspace> res = await NetworkTask.RequestAsync<SimpleWorkspace>(url, eMethodType.GET, string.Empty, token);
            return string.IsNullOrEmpty(res.message) ? string.Empty : res.message; 
        }

        // step 3. join a world
        private async UniTask<string> JoinWorldAsync(int workspaceSeq) 
        {
            string url = string.Format(URL.JOIN_MEMBER, workspaceSeq);
            string token = R.singleton.token;
            ReqWorkspaceSeq info = new ReqWorkspaceSeq(workspaceSeq);
            string body = JsonUtility.ToJson(info);

            PsResponse<string> res = await NetworkTask.RequestAsync<string>(url, eMethodType.POST, body, token);
            return string.IsNullOrEmpty(res.message) ? string.Empty : res.message;
        }

        // step 4. get member seq
        private async UniTask<PsResponse<ResWorkspaceInfo>> GetWorkspaceInfoAsync(int workspaceSeq) 
        {
            string token = R.singleton.token;
            PsResponse<ResWorkspaceList> res = await NetworkTask.RequestAsync<ResWorkspaceList>(URL.WORKSPACE_LIST, eMethodType.GET, string.Empty, token);
            if (! string.IsNullOrEmpty(res.message))
            { 
                return new PsResponse<ResWorkspaceInfo>(-1, res.message);
            }

            PsResponse<ResWorkspaceInfo> result = null;
            foreach (ResWorkspaceInfo info in res.data.list) 
            {
                if (info.workspace.seq == workspaceSeq) 
                {
                    result = new PsResponse<ResWorkspaceInfo>(200, info);
                    break;
                }
            }

            return result;
        }

    #endregion  // world functions
    }
}