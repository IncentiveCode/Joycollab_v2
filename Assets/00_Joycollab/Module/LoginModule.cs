/// <summary>
/// Login 기능만 독립적으로 분리한 모듈
/// @author         : HJ Lee
/// @last update    : 2023. 05. 10
/// @version        : 0.2
/// @update
///     v0.1 (2023. 05. 09) : 최초 생성
///     v0.2 (2023. 05. 10) : LoginScene 에서 사용하던 Login 관련 기능 정리.
/// </summary>

using UnityEngine;
using Cysharp.Threading.Tasks;

namespace Joycollab.v2
{
    public class LoginModule : MonoBehaviour
    {
        private const string TAG = "LoginModule";


    #region public functions

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

    #endregion  // public functions
    }
}